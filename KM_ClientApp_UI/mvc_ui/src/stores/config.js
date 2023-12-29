import { useMyFetch } from '@/shared/useMyFetch'

import { acceptHMRUpdate, defineStore } from 'pinia'
import { ref } from 'vue'

export const useConfigStore = defineStore('config', () => {
  const appConfig = ref(null)

  async function InitAppConfigAsync() {
    const { data } = await useMyFetch('config').get().json()
    if (data.value !== null) {
      appConfig.value = data.value.data.configurations
    }
  }

  return { appConfig, InitAppConfigAsync }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useConfigStore, import.meta.hot))
}
