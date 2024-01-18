import { useMyFetch } from '@/shared/useMyFetch.js'

import { useSessionStore } from '@/stores/session.js'

import { acceptHMRUpdate, defineStore } from 'pinia'
import { useTimeout, promiseTimeout } from '@vueuse/core'
import { ref } from 'vue'
import { useContentStore } from '@/stores/content.js'

export const useFeedbackStore = defineStore('feedback', () => {
  const sessionStore = useSessionStore()
  const contentStore = useContentStore()

  const windowOption = ref({
    isOpen: false,
    isError: false,
    isSending: false
  })

  const rating = ref({
    value: 0,
    showErr: false,
    errMsg: ''
  })

  const remark = ref({
    value: '',
    showErr: false,
    errMsg: ''
  })

  const ratingStars = ref([
    { key: 1, isHover: false },
    { key: 2, isHover: false },
    { key: 3, isHover: false },
    { key: 4, isHover: false }
  ])

  function SetRemarkError(errMsg) {
    remark.value.errMsg = errMsg
    remark.value.showErr = true
  }

  function SetRatingError(errMsg) {
    rating.value.errMsg = errMsg
    rating.value.showErr = true
  }

  function ResetError() {
    rating.value.showErr = false
    rating.value.errMsg = ''

    remark.value.showErr = false
    remark.value.errMsg = ''
  }

  async function SendValidator() {
    if (rating.value.value === 0 && remark.value.value === '') {
      SetRatingError('*Required')
      SetRemarkError('*Required')
      return false
    }

    if (rating.value.value < 3 && remark.value.value === '') {
      SetRemarkError('*Please provide your reason')
      return false
    }

    ResetError()
    return true
  }

  async function SendFeedback() {
    const isValid = await SendValidator()
    if (isValid) {
      const PAYLOAD = JSON.stringify({
        Session_Id: sessionStore.userSession.id,
        Rating: rating.value.value,
        Remark: remark.value.value
      })

      windowOption.value.isSending = true
      windowOption.value.isOpen = false
      const { statusCode } = await useMyFetch('feedback').post(PAYLOAD).json()
      windowOption.value.isError = statusCode.value !== 204

      const ready = useTimeout(1000)
      await promiseTimeout(2000)

      if (ready.value) {
        windowOption.value.isSending = false

        if (statusCode.value === 204) {
          await contentStore.FeedbackContent().then(() => {
            sessionStore.userSession.has_feedback = true
          })
        }
      }
    }
  }

  return { windowOption, rating, remark, ratingStars, SendFeedback }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useFeedbackStore, import.meta.hot))
}