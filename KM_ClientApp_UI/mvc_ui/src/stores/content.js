import {
  GetCategoriesAsync,
  GetReferenceCategoriesAsync,
  PostHeatSelectedCategory,
  SearchCategoriesAsync,
  SuggestCategoriesAsync
} from '@/api/categories'
import { GetContentByIdAsync } from '@/api/content'
import { GetMessageByTypeAsync, MessageType } from '@/api/message'

import { useConfigStore } from '@/stores/config'
import { useSessionStore } from '@/stores/session'

import { promiseTimeout, useTimeout } from '@vueuse/core'
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
        sessionStore.recordHandler.markSelectedCategory(createAt)
        return await ResponseSelectedContent(id)
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
      return await Render.Category(categoriesResponse.categories)
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

      return await ResponseSelectedContent(id)
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
      return await Render.Category(categoriesResponse.categories)
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

    return await Render.MessageWithCategory(messageResponse.messages, categoriesResponse.categories)
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
      return await NotFoundCategoryContent(searchedKeyword, pageNum)
    }

    if (categoriesResponse.is_single) {
      const contentId = categoriesResponse.categories.items[0].id
      const message = messageResponse.messages

      return await SingleResponseContent(contentId, message)
    }

    sessionStore.recordHandler.markSelectedCategory()
    return await Render.MessageWithCategory(messageResponse.messages, categoriesResponse.categories)
  }

  async function SuggestedCategoryContent(userInput, pageNum) {
    const response = await Promise.allSettled([
      GetMessageByTypeAsync(MessageType.suggestion),
      SuggestCategoriesAsync(pageNum)
    ])
    const messageResponse = response[0].value
    const categoriesResponse = response[1].value

    if (!categoriesResponse.is_success || !messageResponse.is_success) {
      return await ShowErrorContent()
    }

    sessionStore.recordHandler.addUserMessage(null, userInput)
    sessionStore.recordHandler.markSelectedCategory()
    return await Render.MessageWithCategory(messageResponse.messages, categoriesResponse.categories)
  }

  async function NotFoundCategoryContent(searchedKeyword, pageNum) {
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

    return await Render.MessageWithCategory(messages, categoriesResponse.categories)
  }

  async function ResponseSelectedContent(contentId) {
    const contentResponse = await GetContentByIdAsync(contentId)
    const messageResponse = await GetMessageByTypeAsync(MessageType.solved)

    if (!contentResponse.is_success || !messageResponse.is_success) {
      return await ShowErrorContent()
    }

    return await Render.Response(contentResponse.content, messageResponse.messages, null)
  }

  async function SingleResponseContent(contentId, singleMessage) {
    const contentResponse = await GetContentByIdAsync(contentId)
    const messageResponse = await GetMessageByTypeAsync(MessageType.solved)

    if (!contentResponse.is_success || !messageResponse.is_success) {
      return await ShowErrorContent()
    }
    return await Render.Response(contentResponse.content, messageResponse.messages, singleMessage)
  }

  async function EndConversationContent(endedBy = null) {
    const messageResponse = await GetMessageByTypeAsync(MessageType.closing)

    if (!messageResponse.is_success) {
      return await ShowErrorContent()
    }

    sessionStore.recordHandler.addUserMessage(null, 'No')
    sessionStore.recordHandler.markSelectedCategory()

    await Render.Message(messageResponse.messages)

    sessionStore.userSession.is_active = false
    return await sessionStore.sessionHandler.end(endedBy)
  }

  const Render = {
    Category: async (categories) => {
      sessionStore.recordHandler.addBotCategory(categories)
      await sessionStore.sessionHandler.update()
    },
    Message: async (messages) => {
      for (let i = 0; i <= messages.length - 1; i++) {
        const ready = useTimeout(delayTyping * i)
        const message = messages[i]
        sessionStore.recordHandler.addBotMessage(message)
        await promiseTimeout(delayTyping)
        if (ready.value) {
          sessionStore.recordHandler.markAsRendered()
        }
      }
      await sessionStore.sessionHandler.update()
    },
    MessageWithCategory: async (messages, categories) => {
      for (let i = 0; i <= messages.length - 1; i++) {
        const ready = useTimeout(delayTyping * i)
        const message = messages[i]
        sessionStore.recordHandler.addBotMessage(message)
        await promiseTimeout(delayTyping)
        if (ready.value) {
          sessionStore.recordHandler.markAsRendered()
        }
      }
      sessionStore.recordHandler.addBotCategory(categories)
      await sessionStore.sessionHandler.update()
    },
    Response: async (content, endMessage, singleMessage) => {
      const { ready, start } = useTimeout(delayTyping, { controls: true })
      sessionStore.recordHandler.addBotContent(content, singleMessage)
      await promiseTimeout(delayTyping)
      if (ready.value) {
        sessionStore.recordHandler.markAsRendered()
        start()
      }

      await promiseTimeout(delayTyping * 4)
      endMessage?.map((msg) => (msg.type = 'message'))
      for (let i = 0; i <= endMessage.length - 1; i++) {
        start()
        const message = endMessage[i]
        sessionStore.recordHandler.addBotMessage(message)
        await promiseTimeout(delayTyping)
        if (ready.value) {
          sessionStore.recordHandler.markAsRendered()
        }
      }

      sessionStore.recordHandler.addBotCategory({ is_closed: true })
      await sessionStore.sessionHandler.update()
    }
  }

  return {
    isFocused,
    isSystemErr,
    StartUpContent,
    ShowErrorContent,
    Common,
    Searched,
    SearchedCategoryContent,
    SuggestedCategoryContent,
    EndConversationContent
  }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useContentStore, import.meta.hot))
}
