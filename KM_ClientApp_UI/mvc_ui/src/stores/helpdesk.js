import { acceptHMRUpdate, defineStore } from 'pinia'
import { ref } from 'vue'
import { useMyFetch } from '@/shared/useMyFetch.js'
import { useSessionStore } from '@/stores/session.js'
import { promiseTimeout, useTimeout } from '@vueuse/core'
import { useContentStore } from '@/stores/content.js'

export const useHelpdeskStore = defineStore('helpdesk', () => {
  const sessionStore = useSessionStore()
  const contentStore = useContentStore()

  const windowOption = ref({
    isOpen: false,
    isError: false,
    isSending: false
  })

  const userMessage = ref({
    content: '',
    showErr: false,
    errMsg: ''
  })

  function openWindow() {
    windowOption.value.isOpen = true
  }

  function closeWindow() {
    windowOption.value.isOpen = false
  }

  async function SendMailHelpdesk() {
    if (userMessage.value.content === '') {
      userMessage.value.showErr = true
      userMessage.value.errMsg = 'Required'
      return
    }

    const PAYLOAD = JSON.stringify({
      Session_Id: sessionStore.userSession.id,
      Message: userMessage.value.content
    })

    windowOption.value.isSending = true
    windowOption.value.isOpen = false
    const { statusCode } = await useMyFetch('email/helpdesk').post(PAYLOAD).json()

    if (statusCode.value !== 204) {
      windowOption.value.isError = true
      return
    }

    const ready = useTimeout(1000)
    await promiseTimeout(2000)

    if (ready.value) {
      windowOption.value.isSending = false
      userMessage.value.content = ''
      userMessage.value.errMsg = ''
      userMessage.value.showErr = false

      return await contentStore.HelpdeskContent()
    }
  }

  return { windowOption, userMessage, openWindow, closeWindow, SendMailHelpdesk }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useHelpdeskStore, import.meta.hot))
}