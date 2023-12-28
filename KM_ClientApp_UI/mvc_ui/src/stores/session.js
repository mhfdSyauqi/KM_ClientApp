import { useMyFetch } from '@/shared/useMyFetch'
import { acceptHMRUpdate, defineStore } from 'pinia'
import { ref } from 'vue'

export const useSessionStore = defineStore('session', () => {
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
        return await sessionHandler.get()
      }

      const { id, is_active, has_feedback, records } = data.value.data.session
      userSession.value.id = id
      userSession.value.is_active = is_active
      userSession.value.has_feedback = has_feedback
      userSession.value.records = JSON.parse(records)
    },
    create: async () => {
      const { statusCode, error } = await useMyFetch('session').post()
      if (statusCode !== 201) {
        // TODO : Create Error Handler Method
      }
    },
    update: async () => {
      const patchSessionRequest = {
        Id: userSession.value.id,
        New_Records: JSON.stringify(userSession.value.records)
      }

      const payload = JSON.stringify(patchSessionRequest)
      const { statusCode, error } = await useMyFetch('session').patch(payload, 'application/json')
      if (statusCode !== 204) {
        // TODO : Create Error Handler Method
      }
    },
    end: async (endedBy = null) => {
      const endSessionRequest = {
        Id: userSession.value.id,
        Ended_By: endedBy
      }

      const payload = JSON.stringify(endSessionRequest)
      const { statusCode, error } = await useMyFetch('session').delete(payload, 'application/json')

      if (statusCode !== 204) {
        // TODO : Create Error Handler Method
      }
    }
  }

  const recordHandler = {
    addUserMessage: async (categoryId, categoryName) => {
      const userRecord = {
        time: new Date().toISOString(),
        message: {
          text: categoryName
        },
        uid: categoryId,
        actor: 'user'
      }
      userSession.value.records.push(userRecord)
    },
    addBotMessage: async (type, response) => {
      let botRecord = {
        time: new Date().toISOString(),
        actor: 'bot',
        rendered: false
      }

      if (type === 'category') {
        botRecord.categories = response
        botRecord.categories.selected = false
      } else {
        botRecord.message = response
      }

      userSession.value.records.push(botRecord)
    },
    markSelectedCategory: async (createAt = null) => {
      userSession.value.records.map((record) => {
        if (createAt !== null && record.categories && record.time === createAt) {
          record.categories.selected = true
        }

        if (createAt === null && record.categories) {
          record.categories.selected = true
        }
      })
    },
    markAsRendered: async () => {
      userSession.value.records.map((record) => {
        if (record.type !== 'category' && record.rendered === false) {
          record.rendered = !record.selected
        }
      })
    }
  }

  return { userSession, sessionHandler, recordHandler }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useSessionStore, import.meta.hot))
}
