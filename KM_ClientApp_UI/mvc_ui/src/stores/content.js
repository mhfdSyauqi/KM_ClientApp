import { GetCategoriesAsync, GetReferenceCategoriesAsync } from '@/api/categories'
import { GetMessageByTypeAsync, MessageType } from '@/api/message'
import { useConfigStore } from '@/stores/config'
import { useSessionStore } from '@/stores/session'
import { promiseTimeout, useArrayFilter, useTimeout } from '@vueuse/core'
import { acceptHMRUpdate, defineStore } from 'pinia'
import { ref } from 'vue'

export const useContentStore = defineStore('content', () => {
  const configStore = useConfigStore()
  const sessionStore = useSessionStore()
  const delayTyping = configStore.appConfig?.delay_typing ?? 500

  const inputFocused = ref(false)

  async function StartUpContent() {
    await ResponseLayeredContent({ messageType: MessageType.welcome })
  }

  async function SelectedCategoryContent(categoryObj, targetedLayer, createdAt) {
    const { id, name, has_content } = categoryObj

    if (has_content) {
      return // Do Content Result
    }
    await sessionStore.recordHandler.addUserMessage(id, name)
    await sessionStore.recordHandler.markSelectedCategory(createdAt)

    // Telemetry Selected Categories

    const MSG_TYPE =
      targetedLayer === 3
        ? MessageType.layer_three
        : targetedLayer === 2
          ? MessageType.layer_two
          : MessageType.layer_one

    await ResponseLayeredContent({ searchedId: id, searchedName: name, messageType: MSG_TYPE })
  }

  async function LoadMoreLayeredContent(currentSearchedId, nextPage, createdAt) {
    if (nextPage > 2 || nextPage === null) {
      // Optional Give Message And User Flow
      return (inputFocused.value = !inputFocused.value)
    }

    const categoriesResponse = await GetCategoriesAsync(currentSearchedId, nextPage)
    if (!categoriesResponse.is_success) {
      return // Do Session Record Error
    }

    await sessionStore.recordHandler.markSelectedCategory(createdAt)
    sessionStore.recordHandler.addBotMessage('category', categoriesResponse.categories)

    const arrNew = useArrayFilter(sessionStore.userSession.records, (item) =>
      item.categories ? item.categories.selected === false : item
    )
    sessionStore.userSession.records = arrNew.value

    return sessionStore.sessionHandler.update()
  }

  async function BackToContent(currentSearchedId, targetedLayer, prevPage, createdAt) {
    if (targetedLayer > 1 && prevPage === null) {
      const refResponse = await GetReferenceCategoriesAsync(currentSearchedId)
      if (!refResponse.is_success) {
        // Do Something
      }

      const categoryObj = {
        id: refResponse.reference,
        name: targetedLayer === 3 ? 'Menu Sebelumnya' : 'Menu Utama',
        has_content: false
      }

      return await SelectedCategoryContent(categoryObj, targetedLayer, createdAt)
    }
    return await LoadMoreLayeredContent(currentSearchedId, prevPage, createdAt)
  }

  async function BackToMainMenu(currentLayer, createdAt) {
    if (currentLayer > 1) {
      const categoryObj = {
        id: null,
        name: 'Menu Utama',
        has_content: false
      }

      return await SelectedCategoryContent(categoryObj, 1, createdAt)
    }
  }

  async function ResponseLayeredContent({ searchedId, searchedName, pageNum, messageType }) {
    //const response = Promise.all(values)

    const categoriesResponse = await GetCategoriesAsync(searchedId, pageNum)
    const messageResponse = await GetMessageByTypeAsync(messageType, searchedName)

    if (!categoriesResponse.is_success || !messageResponse.is_success) {
      return // Do Session Record Error
    }

    for (let i = 0; i <= messageResponse.messages.length - 1; i++) {
      const ready = useTimeout(delayTyping * i)
      const message = messageResponse.messages[i]
      sessionStore.recordHandler.addBotMessage('message', message)
      await promiseTimeout(delayTyping)
      if (ready.value) {
        sessionStore.recordHandler.markAsRendered()
      }
    }
    sessionStore.recordHandler.addBotMessage('category', categoriesResponse.categories)

    const arrNew = useArrayFilter(sessionStore.userSession.records, (item) =>
      item.categories ? item.categories.selected === false : item
    )
    sessionStore.userSession.records = arrNew.value

    return sessionStore.sessionHandler.update()
  }

  return {
    inputFocused,

    StartUpContent,
    SelectedCategoryContent,
    LoadMoreLayeredContent,
    BackToContent,
    BackToMainMenu
  }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useContentStore, import.meta.hot))
}
