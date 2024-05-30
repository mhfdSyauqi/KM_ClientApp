import { useMyFetch } from '@/shared/useMyFetch.js'

import { useSessionStore } from '@/stores/session.js'

import { acceptHMRUpdate, defineStore } from 'pinia'
import { useTimeout, promiseTimeout, useArrayMap } from '@vueuse/core'
import { ref } from 'vue'
import { useContentStore } from '@/stores/content.js'
import { useConfigStore } from '@/stores/config.js'

export const useFeedbackStore = defineStore('feedback', () => {
  const sessionStore = useSessionStore()
  const contentStore = useContentStore()
  const configStore = useConfigStore()

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

    if (remark.value.value === '') {
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
          await contentStore.FeedbackContent().then(async () => {
            sessionStore.userSession.has_feedback = true
            await SendMailHistory()
          })
        }
      }
    }
  }

  async function SendMailHistory() {
    const records = useArrayMap(sessionStore.userSession.records, (item) => {
      if (item.actor === 'bot' && item.message) {
        return {
          Actor: item.actor,
          Time: item.time,
          Message: item.message.text,
          Content: null,
          Link: null
        }
      }
      if (item.actor === 'bot' && item.content) {
        return {
          Actor: 'content',
          Time: item.time,
          Message: item.content.message?.text,
          Content: item.content.description,
          Link: configStore.appConfig.article_link + '/' + item.content.id
        }
      }
      if (item.actor === 'user') {
        return {
          Actor: item.actor,
          Time: item.time,
          Message: item.message?.text,
          Content: null,
          Link: null
        }
      }
    })

    const PAYLOAD = JSON.stringify({
      Histories: records.value
    })

    await useMyFetch('email').post(PAYLOAD).json()
  }

  return { windowOption, rating, remark, ratingStars, SendFeedback }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useFeedbackStore, import.meta.hot))
}