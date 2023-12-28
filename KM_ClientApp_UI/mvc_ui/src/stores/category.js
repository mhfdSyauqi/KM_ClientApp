import { GetCategoriesAsync } from '@/api/categories'
import { GetMessageByTypeAsync, MessageType } from '@/api/message'
import { useConfigStore } from '@/stores/config'
import { useSessionStore } from '@/stores/session'
import { promiseTimeout, useTimeout } from '@vueuse/core'
import { acceptHMRUpdate, defineStore } from 'pinia'

export const useCategoryStore = defineStore('category', () => {
  const SelectedCategory = async (category, time) => {
    const sessionStore = useSessionStore()
    const configStore = useConfigStore()
    const delayTyping = configStore.appConfig?.delay_typing ?? 500

    const { id, name, has_content } = category
    if (has_content) {
      return await SelectedContent(id)
    }
    await sessionStore.recordHandler.addUserMessage(id, name)

    await sessionStore.recordHandler.markSelectedCategory(time)

    const { is_success: has_sub, categories } = await GetCategoriesAsync(id)

    if (!has_sub) {
      // DO Something
    }

    const MSG_TYPE =
      categories.layer === 3
        ? MessageType.layer_three
        : categories.layer === 2
          ? MessageType.layer_two
          : MessageType.layer_one

    const { is_success: has_msg, messages } = await GetMessageByTypeAsync(MSG_TYPE, name)

    if (!has_msg) {
      // DO Something
    }

    for (let i = 0; i <= messages.length - 1; i++) {
      const ready = useTimeout(delayTyping * i)
      const message = messages[i]
      sessionStore.recordHandler.addBotMessage('message', message)
      await promiseTimeout(delayTyping)
      if (ready.value) {
        sessionStore.recordHandler.markAsRendered()
      }
    }

    await sessionStore.recordHandler.addBotMessage('category', categories)

    // TELEMETRY

    return await sessionStore.sessionHandler.update()
  }

  const SelectedContent = async (categoryId) => {
    // Do Somthing
  }

  return { SelectedCategory }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useCategoryStore, import.meta.hot))
}
