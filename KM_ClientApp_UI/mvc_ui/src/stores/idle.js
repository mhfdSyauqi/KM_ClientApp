import { GetMessageByTypeAsync, MessageType } from '@/api/message.js'

import { acceptHMRUpdate, defineStore } from 'pinia'

import { useSessionStore } from '@/stores/session.js'
import { promiseTimeout, useTimeout } from '@vueuse/core'
import { useConfigStore } from '@/stores/config.js'
import { useContentStore } from '@/stores/content.js'
import { ref } from 'vue'

export const useIdleStore = defineStore('idle', () => {
  const configStore = useConfigStore()
  const sessionStore = useSessionStore()
  const contentStore = useContentStore()

  const delayTyping = configStore.appConfig?.delay_typing ?? 500

  const Props = ref({
    attempt: configStore.appConfig?.idle_attempt,
    duration: configStore.appConfig?.idle_duration,
    $reset: () => {
      Props.value.attempt = configStore.appConfig?.idle_attempt
    }
  })

  async function IdleHandler(idleAttempt, resetCallback) {
    const closingMsg = await GetMessageByTypeAsync(MessageType.closing)
    const idleMsg = await GetMessageByTypeAsync(MessageType.idle)
    if (!idleMsg.is_success && !closingMsg.is_success) {
      return await contentStore.ShowErrorContent()
    }

    resetCallback()
    Props.value.attempt -= 1
    sessionStore.recordHandler.markSelectedCategory()

    if (idleAttempt === 0) {
      for (let i = 0; i <= closingMsg.messages.length - 1; i++) {
        const ready = useTimeout(delayTyping * i)
        const message = closingMsg.messages[i]
        sessionStore.recordHandler.addBotMessage(message)
        await promiseTimeout(delayTyping)
        if (ready.value) {
          sessionStore.recordHandler.markAsRendered()
        }
      }

      await sessionStore.sessionHandler.update()

      sessionStore.userSession.is_active = false
      return await sessionStore.sessionHandler.end('System')
    }

    for (let i = 0; i <= idleMsg.messages.length - 1; i++) {
      const ready = useTimeout(delayTyping * i)
      const message = idleMsg.messages[i]
      sessionStore.recordHandler.addBotMessage(message)
      await promiseTimeout(delayTyping)
      if (ready.value) {
        sessionStore.recordHandler.markAsRendered()
      }
    }
    sessionStore.recordHandler.addBotCategory({ is_idle: true })
    await sessionStore.sessionHandler.update()
  }

  return { Props, IdleHandler }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useIdleStore, import.meta.hot))
}