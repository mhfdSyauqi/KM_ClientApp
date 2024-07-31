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
        //iframe.value.src = 'https://localhost:44356/'
        iframe.value.src = 'http://localhost:5173/'
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
    <iframe class="w-full h-full border-0" ref="iframe" src="about:blank"></iframe>

    <div
      class="w-full absolute right-0 top-0 flex h-[11%] xl:h-[10%] justify-end items-center px-7"
    >
      <a
        class="cursor-pointer bg-transparent border-0 active:scale-95"
        @click="configStore.windowInfo.toggle"
      >
        <IconClose class="fill-white" />
      </a>
    </div>
  </div>
</template>

<style scoped></style>
