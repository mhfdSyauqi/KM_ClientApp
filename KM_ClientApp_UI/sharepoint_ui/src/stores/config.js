import { acceptHMRUpdate, defineStore } from 'pinia'
import { reactive } from 'vue'

export const useConfigStore = defineStore('config', () => {
  const windowInfo = reactive({
    isOpen: false,
    toggle() {
      this.isOpen = !this.isOpen
    }
  })

  const appConfig = reactive({
    app_name: 'Chat Bot',
    app_image: ''
  })

  async function InitAppConfig() {
    const apiOption = await fetch('https://localhost:44356/api/config', {
      method: 'GET',
      credentials: 'include'
    })

    if (apiOption.status === 200) {
      const response = await apiOption.json()

      appConfig.app_image = response.data.configurations.app_image
      appConfig.app_name = response.data.configurations.app_name
    }
  }

  return { windowInfo, appConfig, InitAppConfig }
})

if (import.meta.hot) {
  import.meta.hot.accept(acceptHMRUpdate(useConfigStore, import.meta.hot))
}
