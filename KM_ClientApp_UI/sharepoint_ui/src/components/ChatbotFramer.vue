<script setup>
import IconClose from '@components/icon/IconClose.vue'

import { useConfigStore } from '@stores/config'

import { onMounted, ref } from 'vue'

const configStore = useConfigStore()
const iframe = ref(null)

onMounted(() => {
  iframe.value.src = 'http://localhost:5173'

  // Implement This Later
  // await RenderIframe()
})

async function RenderIframe() {
  const apiOption = await fetch('http://localhost:5173', {
    method: 'GET',
    credentials: 'include'
  })

  iframe.value.contentDocument.write('<h1>Hello World</h1>')
}
</script>

<template>
  <div
    class="fixed w-screen h-screen lg:h-4/5 lg:w-1/3 right-0 bottom-0 bg-gray-100 flex flex-col flex-nowrap drop-shadow-sm"
  >
    <iframe ref="iframe" src="about:blank"></iframe>

    <a class="cursor-pointer absolute right-8 top-5" @click="configStore.windowInfo.toggle">
      <IconClose class="fill-gray-950" />
    </a>
  </div>
</template>

<style scoped>
iframe {
  @apply w-full h-full border-0;
}
</style>
