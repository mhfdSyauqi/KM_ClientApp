<script setup>
import IconClose from '@components/icon/IconClose.vue'

import { useConfigStore } from '@stores/config'

import { onMounted, ref } from 'vue'

const configStore = useConfigStore()
const iframe = ref(null)

onMounted(async () => {
  iframe.value.src = 'https://mhfdsyauqi.github.io/km_statis/'

  // Implement This Later
  // await RenderIframe()
})

async function RenderIframe() {
  const apiOption = await fetch('https://mhfdsyauqi.github.io/km_statis/', {
    method: 'GET',
    credentials: 'include'
  })

  iframe.value.contentDocument.write('<h1>Hello World</h1>')

  console.log(apiOption)
}
</script>

<template>
  <div
    class="fixed w-screen h-screen md:w-1/3 md:h-4/5 lg:w-1/3 right-0 bottom-0 bg-gray-100 flex flex-col flex-nowrap drop-shadow-sm"
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
