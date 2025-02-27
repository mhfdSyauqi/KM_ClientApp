import { useMyFetch } from '@/shared/useMyFetch'

import { useContentStore } from '@/stores/content'
import { useConfigStore } from '@/stores/config.js'

import { acceptHMRUpdate, defineStore } from 'pinia'
import { useArrayFilter } from '@vueuse/core'
import { ref } from 'vue'
import { nanoid } from 'nanoid'

export const useSessionStore = defineStore('session', () => {
  const contentStore = useContentStore()
  const configStore = useConfigStore()

  const userSession = ref({
    id: null,
    is_active: null,
    has_feedback: null,
    records: []
  })

  const sessionHandler = {
    get: async () => {
      const { data, statusCode } = await useMyFetch('session').get().json()
      if (statusCode.value === 404 && data.value === null) {
        await sessionHandler.create()
        await sessionHandler.get()
        return
      }

      const { id, is_active, has_feedback, records } = data.value.data.session
      userSession.value.id = id
      userSession.value.is_active = is_active
      userSession.value.has_feedback = has_feedback
      userSession.value.records = JSON.parse(records)
    },
    create: async () => {
      const { statusCode } = await useMyFetch('session').post()
      if (statusCode.value !== 201) {
        return await contentStore.ShowErrorContent()
      }
    },
    update: async () => {
      const arrNew = useArrayFilter(userSession.value.records, (item) =>
        item.categories ? item.categories.selected === false : item
      )
      userSession.value.records = arrNew.value

      const patchSessionRequest = {
        Id: userSession.value.id,
        New_Records: JSON.stringify(userSession.value.records)
      }

      const payload = JSON.stringify(patchSessionRequest)
      const { statusCode } = await useMyFetch('session').patch(payload, 'application/json')
      if (statusCode.value !== 204) {
        return await contentStore.ShowErrorContent()
      }
    },
    end: async (endedBy = null) => {
      const endSessionRequest = {
        Id: userSession.value.id,
        Ended_By: endedBy
      }

      const payload = JSON.stringify(endSessionRequest)
      const { statusCode } = await useMyFetch('session').delete(payload, 'application/json')
      if (statusCode.value !== 204) {
        return await contentStore.ShowErrorContent()
      }
    }
  }

  const recordHandler = {
    addUserMessage: (categoryId, categoryName) => {
      const userRecord = {
        id: nanoid(10),
        time: new Date().toISOString(),
        message: {
          text: categoryName
        },
        uid: categoryId,
        actor: 'user'
      }
      userSession.value.records.push(userRecord)
    },
    addBotMessage: (messages) => {
      const botMessage = {
        id: nanoid(10),
        time: new Date().toISOString(),
        actor: 'bot',
        message: messages,
        rendered: false
      }
      userSession.value.records.push(botMessage)
    },
    addBotCategory: (categories) => {
      const botCategory = {
        id: nanoid(10),
        time: new Date().toISOString(),
        actor: 'bot',
        categories: {
          selected: false,
          ...categories
        }
      }
      userSession.value.records.push(botCategory)
    },
    addErrorMessage: () => {
      let errRecord = {
        id: nanoid(10),
        time: new Date().toISOString(),
        actor: 'error',
        rendered: false
      }
      userSession.value.records.push(errRecord)
    },
    addBotContent: (responseContent, singleMessage = null) => {
      let contentRecord = {
        id: nanoid(10),
        time: new Date().toISOString(),
        actor: 'bot',
        content: {
          ...responseContent,
          messages: singleMessage?.length > 0 ? singleMessage : []
        },
        rendered: false
      }
      userSession.value.records.push(contentRecord)
    },
    markSelectedCategory: (createAt = null) => {
      userSession.value.records.map((record) => {
        if (createAt !== null && record.categories && record.time === createAt) {
          record.categories.selected = true
        }

        if (createAt === null && record.categories) {
          record.categories.selected = true
        }
      })
    },
    markAsRendered: () => {
      userSession.value.records.map((record) => {
        if (!record.categories && record.rendered === false) {
          record.rendered = !record.selected
        }
      })
      configStore.appAudio.element.currentTime = 0
      configStore.appAudio.element.play()
    }
  }

  return { userSession, sessionHandler, recordHandler }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useSessionStore, import.meta.hot))
}