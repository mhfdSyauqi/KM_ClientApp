<script setup>
import { useConfigStore } from '@stores/config'

import appIcon from '@/assets/appIcon.jpg'

import tippy from 'tippy.js'
import 'tippy.js/dist/tippy.css'
import { onMounted, ref } from 'vue'

const configStore = useConfigStore()
const bubble = ref(null)

onMounted(async () => {
  await configStore.InitAppConfig()
  tippy(bubble.value, {
    content: configStore.appConfig.app_name,
    placement: 'left',
    offset: [0, 30],
    touch: false
  })
})

function LoadAlterNative(e) {
  e.target.src = appIcon
}
</script>

<template>
  <div
    ref="bubble"
    class="fixed w-14 h-14 md:w-16 md:h-16 right-7 bottom-7 drop-shadow-lg z-50"
    :class="[configStore.windowInfo.isOpen ? 'open-bot-anim' : 'close-bot-anim']"
    @click="configStore.windowInfo.toggle"
  >
    <img
      @error.prevent="LoadAlterNative"
      class="rounded-full image-holder"
      :src="configStore.appConfig.app_image"
      alt="app-icon"
    />
  </div>
</template>

<style scoped>
.open-bot-anim {
  animation: open-bounce 0.3s 1 linear forwards;
}

.close-bot-anim {
  animation: close-bounce 0.5s 1 linear forwards;
}

@keyframes open-bounce {
  0%,
  90% {
    transform: translateY(-25%);
    animation-timing-function: cubic-bezier(0.8, 0, 1, 1);
  }
  50% {
    transform: translateY(0);
    animation-timing-function: cubic-bezier(0, 0, 0.2, 1);
  }
  100% {
    opacity: 0;
    display: none;
    animation-timing-function: cubic-bezier(0.8, 0, 1, 1);
  }
}

@keyframes close-bounce {
  0% {
    display: block;
    opacity: 0.1;
  }
  50% {
    opacity: 0.4;
    transform: translateY(20%);
    animation-timing-function: cubic-bezier(0, 0, 0.2, 1);
  }
  100% {
    opacity: 1;
    animation-timing-function: cubic-bezier(0.8, 0, 1, 1);
  }
}

.image-holder {
  display: block;
  max-width: 100%;
  height: auto;
}
</style>
