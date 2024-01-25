import audio from '@/assets/message.mp3'

import { useMyFetch } from '@/shared/useMyFetch'

import { acceptHMRUpdate, defineStore } from 'pinia'
import { ref } from 'vue'

export const useConfigStore = defineStore('config', () => {
  const appConfig = ref(null)
  const appAudio = ref({
    element: null,
    source: audio
  })

  async function InitAppConfigAsync() {
    const { data } = await useMyFetch('config').get().json()
    if (data.value !== null) {
      appConfig.value = data.value.data.configurations
    }
  }

  return { appConfig, appAudio, InitAppConfigAsync }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useConfigStore, import.meta.hot))
}