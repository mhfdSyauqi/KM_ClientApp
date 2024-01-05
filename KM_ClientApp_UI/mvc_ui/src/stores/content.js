import {
  GetCategoriesAsync,
  GetReferenceCategoriesAsync,
  PostHeatSelectedCategory,
  SearchCategoriesAsync,
  SuggestCategoriesAsync
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
  const isSystemErr = ref(false)

  const Common = {
    SelectedCategory: async (categoryObj, nextLayer, createAt) => {
      const { id, name, has_content } = categoryObj

      const heatResponse = await PostHeatSelectedCategory(sessionStore.userSession.id, name, id)
      if (!heatResponse.is_success) {
        return await ShowErrorContent()
      }

      if (has_content) {
        return // Do Some Content Rendering
      }

      const msgType =
        nextLayer === 3
          ? MessageType.layer_three
          : nextLayer === 2
            ? MessageType.layer_two
            : MessageType.layer_one

      sessionStore.recordHandler.addUserMessage(id, name)
      sessionStore.recordHandler.markSelectedCategory(createAt)

      await ResponseLayeredContent({ searchedId: id, searchedName: name, messageType: msgType })
    },
    LoadMoreCategory: async (currentSearchedId, nextPage, createAt) => {
      if (nextPage > 2 || nextPage === null) {
        return (isFocused.value = true)
      }

      const categoriesResponse = await GetCategoriesAsync(currentSearchedId, nextPage)
      if (!categoriesResponse.is_success) {
        return await ShowErrorContent()
      }

      sessionStore.recordHandler.markSelectedCategory(createAt)
      sessionStore.recordHandler.addBotCategory(categoriesResponse.categories)

      const arrNew = useArrayFilter(sessionStore.userSession.records, (item) =>
        item.categories ? item.categories.selected === false : item
      )
      sessionStore.userSession.records = arrNew.value

      return await sessionStore.sessionHandler.update()
    },
    GoBackCategory: async (currentSearchedId, currLayer, prevPage, createAt) => {
      if (currLayer > 1 && prevPage === null) {
        const refResponse = await GetReferenceCategoriesAsync(currentSearchedId)
        if (!refResponse.is_success) {
          return await ShowErrorContent()
        }

        const contentObj = {
          id: refResponse.reference,
          name: currLayer === 3 ? 'Menu Sebelumnya' : 'Menu Utama'
        }

        sessionStore.recordHandler.addUserMessage(contentObj.id, contentObj.name)
        sessionStore.recordHandler.markSelectedCategory(createAt)

        return await ResponseLayeredContent({
          searchedId: contentObj.id,
          searchedName: contentObj.name,
          messageType: MessageType.layer_one
        })
      }
      return await Common.LoadMoreCategory(currentSearchedId, prevPage, createAt)
    },
    GoMainMenu: async (currentLayer, createAt) => {
      if (currentLayer > 1) {
        const menuUtamaObj = {
          id: null,
          name: 'Menu Utama'
        }

        sessionStore.recordHandler.addUserMessage(menuUtamaObj.id, menuUtamaObj.name)
        sessionStore.recordHandler.markSelectedCategory(createAt)

        return await ResponseLayeredContent({
          searchedId: menuUtamaObj.id,
          searchedName: menuUtamaObj.name,
          messageType: MessageType.layer_one
        })
      }
    }
  }

  const Searched = {
    SelectedCategory: async (categoryObj, createAt) => {
      const { id, name, has_content } = categoryObj

      const heatResponse = await PostHeatSelectedCategory(sessionStore.userSession.id, name, id)
      if (!heatResponse.is_success || !has_content) {
        return await ShowErrorContent()
      }

      sessionStore.recordHandler.addUserMessage(id, name)
      sessionStore.recordHandler.markSelectedCategory(createAt)

      // Do Some Content Rendering
    },
    LoadMoreCategory: async (currentSearchedKeyword, nextPage, createAt) => {
      if (nextPage === null) {
        return (isFocused.value = true)
      }

      const categoriesResponse = currentSearchedKeyword
        ? await SearchCategoriesAsync(currentSearchedKeyword, nextPage)
        : await SuggestCategoriesAsync(nextPage)

      if (!categoriesResponse.is_success) {
        return await ShowErrorContent()
      }

      sessionStore.recordHandler.markSelectedCategory(createAt)
      sessionStore.recordHandler.addBotCategory(categoriesResponse.categories)

      const arrNew = useArrayFilter(sessionStore.userSession.records, (item) =>
        item.categories ? item.categories.selected === false : item
      )
      sessionStore.userSession.records = arrNew.value

      return await sessionStore.sessionHandler.update()
    },
    GoBackCategory: async (currSearchedKeyword, prevPage, createAt) => {
      if (prevPage !== null) {
        return await Searched.LoadMoreCategory(currSearchedKeyword, prevPage, createAt)
      }
    },
    GoMainMenu: async (createAt) => {
      const menuUtamaObj = {
        id: null,
        name: 'Menu Utama'
      }

      sessionStore.recordHandler.addUserMessage(menuUtamaObj.id, menuUtamaObj.name)
      sessionStore.recordHandler.markSelectedCategory(createAt)

      return await ResponseLayeredContent({
        searchedId: menuUtamaObj.id,
        searchedName: menuUtamaObj.name,
        messageType: MessageType.layer_one
      })
    }
  }

  async function StartUpContent() {
    await ResponseLayeredContent({ messageType: MessageType.welcome })
  }

  async function ShowErrorContent() {
    sessionStore.recordHandler.markSelectedCategory()
    const ready = useTimeout(delayTyping)
    sessionStore.recordHandler.addErrorMessage()
    await promiseTimeout(delayTyping)
    if (ready.value) {
      sessionStore.recordHandler.markAsRendered()
    }
    isSystemErr.value = true
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

  async function SearchedCategoryContent(searchedKeyword, pageNum) {
    const response = await Promise.allSettled([
      SearchCategoriesAsync(searchedKeyword, pageNum),
      GetMessageByTypeAsync(MessageType.searched, searchedKeyword)
    ])

    const categoriesResponse = response[0].value
    const messageResponse = response[1].value

    const heatResponse = await PostHeatSelectedCategory(
      sessionStore.userSession.id,
      searchedKeyword,
      null
    )

    sessionStore.recordHandler.addUserMessage(null, searchedKeyword)

    if (
      (!categoriesResponse.is_success && !categoriesResponse.is_not_found) ||
      !messageResponse.is_success ||
      !heatResponse.is_success
    ) {
      return await ShowErrorContent()
    }

    if (categoriesResponse.is_not_found) {
      return await SuggestedCategoryContent(searchedKeyword, pageNum)
    }

    if (categoriesResponse.single) {
      return // Render Single Content
    }

    sessionStore.recordHandler.markSelectedCategory()

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

  async function SuggestedCategoryContent(searchedKeyword, pageNum) {
    const response = await Promise.allSettled([
      GetMessageByTypeAsync(MessageType.not_found, searchedKeyword),
      SuggestCategoriesAsync(pageNum),
      GetMessageByTypeAsync(MessageType.suggestion)
    ])

    const notFoundResponse = response[0].value
    const categoriesResponse = response[1].value
    const messageResponse = response[2].value

    if (
      !notFoundResponse.is_success ||
      !categoriesResponse.is_success ||
      !messageResponse.is_success
    ) {
      return await ShowErrorContent()
    }

    sessionStore.recordHandler.markSelectedCategory()

    notFoundResponse.messages.map((msg) => (msg.type = 'message'))
    const messages = [...notFoundResponse.messages, ...messageResponse.messages]

    for (let i = 0; i <= messages.length - 1; i++) {
      const ready = useTimeout(delayTyping * i)
      const message = messages[i]
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

  return {
    isFocused,
    isSystemErr,
    StartUpContent,
    ShowErrorContent,
    Common,
    Searched,
    SearchedCategoryContent
  }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useContentStore, import.meta.hot))
}
