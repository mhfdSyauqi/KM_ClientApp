import {
  GetCategoriesAsync,
  GetReferenceCategoriesAsync,
  PostHeatSelectedCategory,
  ReAskedCategoryStatus,
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

      const { count } = await ReAskedCategoryStatus(id)
      const heatResponse = await PostHeatSelectedCategory(sessionStore.userSession.id, name, id)

      if (!heatResponse.is_success) {
        return await ShowErrorContent()
      }

      sessionStore.recordHandler.addUserMessage(id, name)
      sessionStore.recordHandler.markSelectedCategory(createAt)

      if (has_content && count > 0) {
        return await ReAskedSelectedContent(id, name)
      }

      if (has_content && count === 0) {
        return await ResponseSelectedContent(id)
      }

      const msgType =
        nextLayer === 3
          ? MessageType.layer_three
          : nextLayer === 2
            ? MessageType.layer_two
            : MessageType.layer_one

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
      return await Common.LoadMoreCategory(currentSearchedId, prevPage, createAt)
    },
    GoMainMenu: async (currentLayer, createAt) => {
      if (currentLayer > 1) {
        const menuUtamaObj = {
          id: null,
          name: 'Main Menu'
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

      const { count } = await ReAskedCategoryStatus(id)
      const heatResponse = await PostHeatSelectedCategory(sessionStore.userSession.id, name, id)
      if (!heatResponse.is_success || !has_content) {
        return await ShowErrorContent()
      }

      sessionStore.recordHandler.addUserMessage(id, name)
      sessionStore.recordHandler.markSelectedCategory(createAt)

      if (count > 0) {
        return await ReAskedSelectedContent(id, name)
      }

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
        name: 'Main Menu'
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
    if (searchedKeyword.toLowerCase() === 'main menu') {
      return await Searched.GoMainMenu()
    }

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
      return await NotFoundCategoryContent(searchedKeyword)
    }

    sessionStore.recordHandler.markSelectedCategory()

    if (categoriesResponse.is_single) {
      const contentId = categoriesResponse.categories.items[0].id
      const message = messageResponse.messages

      const { count } = await ReAskedCategoryStatus(contentId)
      await PostHeatSelectedCategory(sessionStore.userSession.id, searchedKeyword, contentId)

      if (count > 0) {
        return await ReAskedSelectedContent(contentId, searchedKeyword)
      }

      return await SingleResponseContent(contentId, message)
    }

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

  async function NotFoundCategoryContent(searchedKeyword) {
    const notFoundResponse = await GetMessageByTypeAsync(MessageType.not_found, searchedKeyword)

    if (!notFoundResponse.is_success) {
      return await ShowErrorContent()
    }

    sessionStore.recordHandler.markSelectedCategory()

    return await Render.NotFound(notFoundResponse.messages)
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

  async function FeedbackContent() {
    const messageResponse = await GetMessageByTypeAsync(MessageType.feedback)
    if (messageResponse.is_success) {
      return await Render.Message(messageResponse.messages)
    }
  }

  async function ReAskedSelectedContent(contentId, contentName) {
    const messageResponse = await GetMessageByTypeAsync(MessageType.reasked, contentName)
    if (!messageResponse.is_success) {
      return await ShowErrorContent()
    }

    return await Render.ReAsked(contentId, messageResponse.messages)
  }

  async function ReAskedContent(contentId) {
    await sessionStore.recordHandler.markSelectedCategory()
    await sessionStore.recordHandler.addUserMessage(null, 'Yes')

    return await ResponseSelectedContent(contentId)
  }

  async function EndingReAskedContent() {
    const messageResponse = await GetMessageByTypeAsync(MessageType.solved)
    if (!messageResponse.is_success) {
      return await ShowErrorContent()
    }
    sessionStore.recordHandler.addUserMessage(null, 'No')
    sessionStore.recordHandler.markSelectedCategory()

    return await Render.Ending(messageResponse.messages)
  }

  async function HelpdeskContent() {
    const mailSendResponse = await GetMessageByTypeAsync(MessageType.mail_sended)
    const solvedResponse = await GetMessageByTypeAsync(MessageType.solved)

    if (!mailSendResponse.is_success || !solvedResponse.is_success) {
      return await ShowErrorContent()
    }

    mailSendResponse.messages.map((msg) => (msg.type = 'message'))
    const messages = [...mailSendResponse.messages, ...solvedResponse.messages]

    sessionStore.recordHandler.addUserMessage(null, 'Send Email To Helpdesk')
    sessionStore.recordHandler.markSelectedCategory()

    return await Render.Ending(messages)
  }

  const Render = {
    Category: async (categories) => {
      sessionStore.recordHandler.addBotCategory(categories)
      await sessionStore.sessionHandler.update()
    },
    Message: async (messages) => {
      const ready = useTimeout(delayTyping)
      for (let i = 0; i <= messages.length - 1; i++) {
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
      const ready = useTimeout(delayTyping)
      for (let i = 0; i <= messages.length - 1; i++) {
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
    },
    ReAsked: async (contentId, reAskedMessages) => {
      const ready = useTimeout(delayTyping)
      for (let i = 0; i <= reAskedMessages.length - 1; i++) {
        const message = reAskedMessages[i]
        sessionStore.recordHandler.addBotMessage(message)
        await promiseTimeout(delayTyping)
        if (ready.value) {
          sessionStore.recordHandler.markAsRendered()
        }
      }

      sessionStore.recordHandler.addBotCategory({ is_reasked: true, searched_identity: contentId })
      await sessionStore.sessionHandler.update()
    },
    NotFound: async (notFoundMessage) => {
      const ready = useTimeout(delayTyping)
      for (let i = 0; i <= notFoundMessage.length - 1; i++) {
        const message = notFoundMessage[i]
        sessionStore.recordHandler.addBotMessage(message)
        await promiseTimeout(delayTyping)
        if (ready.value) {
          sessionStore.recordHandler.markAsRendered()
        }
      }

      sessionStore.recordHandler.addBotCategory({ is_not_found: true })
      await sessionStore.sessionHandler.update()
    },
    Ending: async (endMessage) => {
      const ready = useTimeout(delayTyping)
      for (let i = 0; i <= endMessage.length - 1; i++) {
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
    EndConversationContent,
    FeedbackContent,
    ReAskedContent,
    EndingReAskedContent,
    HelpdeskContent
  }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useContentStore, import.meta.hot))
}
