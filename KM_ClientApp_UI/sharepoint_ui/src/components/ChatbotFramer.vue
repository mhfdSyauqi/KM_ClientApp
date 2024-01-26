<script setup>
import IconClose from '@components/icon/IconClose.vue'

import { useConfigStore } from '@stores/config'

import { onMounted, ref, watch } from 'vue'

const configStore = useConfigStore()
const iframe = ref(null)

onMounted(async () => {
  watch(
    configStore.windowInfo,
    (current) => {
      if (current.isOpen && iframe.value.src === 'about:blank') {
        iframe.value.src = 'https://localhost:44356/'
      }
    },
    { deep: true }
  )
})
</script>

<template>
  <div
    class="fixed w-screen h-screen lg:h-4/5 lg:w-1/3 right-0 bottom-0 bg-gray-100 flex flex-col flex-nowrap drop-shadow-sm"
  >
    <iframe ref="iframe" src="about:blank"></iframe>

    <a
      class="cursor-pointer absolute right-8 top-4 xl:top-6"
      @click="configStore.windowInfo.toggle"
    >
      <IconClose class="fill-white" />
    </a>
  </div>
</template>

<style scoped>
iframe {
  @apply w-full h-full border-0;
}
</style>
