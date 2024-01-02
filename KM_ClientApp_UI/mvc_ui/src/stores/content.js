import {
  GetCategoriesAsync,
  GetReferenceCategoriesAsync,
  PostHeatSelectedCategory
} from '@/api/categories'
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

  const isFocused = ref(false)

  async function StartUpContent() {
    await ResponseLayeredContent({ messageType: MessageType.welcome })
  }

  async function SelectedCategoryContent(categoryObj, targetedLayer, createdAt) {
    const { id, name, has_content } = categoryObj

    const heatResponse = await PostHeatSelectedCategory(sessionStore.userSession.id, name, id)

    if (!heatResponse.is_success) {
      return await ShowErrorContent()
    }

    if (has_content) {
      return await ShowErrorContent()
    }
    sessionStore.recordHandler.addUserMessage(id, name)
    sessionStore.recordHandler.markSelectedCategory(createdAt)

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
      return (isFocused.value = true)
    }

    const categoriesResponse = await GetCategoriesAsync(currentSearchedId, nextPage)
    if (!categoriesResponse.is_success) {
      return await ShowErrorContent()
    }

    sessionStore.recordHandler.markSelectedCategory(createdAt)
    sessionStore.recordHandler.addBotCategory(categoriesResponse.categories)

    const arrNew = useArrayFilter(sessionStore.userSession.records, (item) =>
      item.categories ? item.categories.selected === false : item
    )
    sessionStore.userSession.records = arrNew.value

    return await sessionStore.sessionHandler.update()
  }

  async function BackToContent(currentSearchedId, targetedLayer, prevPage, createdAt) {
    if (targetedLayer > 1 && prevPage === null) {
      const refResponse = await GetReferenceCategoriesAsync(currentSearchedId)
      if (!refResponse.is_success) {
        return await ShowErrorContent()
      }

      const contentObj = {
        id: refResponse.reference,
        name: targetedLayer === 3 ? 'Menu Sebelumnya' : 'Menu Utama'
      }

      sessionStore.recordHandler.addUserMessage(contentObj.id, contentObj.name)
      sessionStore.recordHandler.markSelectedCategory(createdAt)

      return await ResponseLayeredContent({
        searchedId: contentObj.id,
        searchedName: contentObj.name,
        messageType: MessageType.layer_one
      })
    }
    return await LoadMoreLayeredContent(currentSearchedId, prevPage, createdAt)
  }

  async function BackToMainMenu(currentLayer, createdAt) {
    if (currentLayer > 1) {
      const menuUtamaObj = {
        id: null,
        name: 'Menu Utama'
      }

      sessionStore.recordHandler.addUserMessage(menuUtamaObj.id, menuUtamaObj.name)
      sessionStore.recordHandler.markSelectedCategory(createdAt)

      return await ResponseLayeredContent({
        searchedId: menuUtamaObj.id,
        searchedName: menuUtamaObj.name,
        messageType: MessageType.layer_one
      })
    }
  }

  async function ResponseLayeredContent({ searchedId, searchedName, pageNum, messageType }) {
    const response = await Promise.allSettled([
      GetCategoriesAsync(searchedId, pageNum),
      GetMessageByTypeAsync(messageType, searchedName)
    ])

    const categoriesResponse = response[0].value
    const messageResponse = response[1].value

    if (!categoriesResponse.is_success || !messageResponse.is_success) {
      return await ShowErrorContent()
    }

    for (let i = 0; i <= messageResponse.messages.length - 1; i++) {
      const ready = useTimeout(delayTyping * i)
      const message = messageResponse.messages[i]
      sessionStore.recordHandler.addBotMessage(message)
      await promiseTimeout(delayTyping)
      if (ready.value) {
        sessionStore.recordHandler.markAsRendered()
      }
    }

    sessionStore.recordHandler.addBotCategory(categoriesResponse.categories)
    const arrNew = useArrayFilter(sessionStore.userSession.records, (item) =>
      item.categories ? item.categories.selected === false : item
    )
    sessionStore.userSession.records = arrNew.value

    return await sessionStore.sessionHandler.update()
  }

  async function ShowErrorContent() {
    sessionStore.recordHandler.markSelectedCategory()
    const ready = useTimeout(delayTyping)
    sessionStore.recordHandler.addErrorMessage()
    await promiseTimeout(delayTyping)
    if (ready.value) {
      sessionStore.recordHandler.markAsRendered()
    }
  }

  return {
    isFocused,
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
