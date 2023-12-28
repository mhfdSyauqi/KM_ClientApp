<script setup>
import appIcon from '@/assets/appIcon.jpg'
import IconDot from '@/components/icons/IconDot.vue'

import { useConfigStore } from '@/stores/config'
const configStore = useConfigStore()
</script>

<template>
  <div class="flex justify-start items-end">
    <div class="basis-[12%]">
      <img
        class="w-[70%] h-[70%] rounded-full"
        :src="configStore.appConfig?.app_image ?? appIcon"
        alt="appIcon"
      />
    </div>
    <div class="basis-auto grow-0 h-full bg-white drop-shadow-lg p-3 rounded-xl rounded-bl-none">
      <div class="flex flex-row justify-start items-end align-middle gap-x-1 h-full">
        <IconDot class="bounce-1 basis-[13%]" />
        <IconDot class="bounce-2 basis-[13%]" />
        <IconDot class="bounce-3 basis-[13%]" />
        <IconDot class="bounce-4 basis-[13%]" />
      </div>
    </div>
  </div>
</template>

<style scoped>
.bounce-1 {
  animation: bounce 0.85s infinite;
}
.bounce-2 {
  animation: bounce 0.85s infinite;
  animation-delay: 30ms;
}
.bounce-3 {
  animation: bounce 0.85s infinite;
  animation-delay: 60ms;
}

.bounce-4 {
  animation: bounce 0.85s infinite;
  animation-delay: 90ms;
}

@keyframes bounce {
  0%,
  100% {
    transform: translateY(-25%);
    animation-timing-function: cubic-bezier(0.8, 0, 1, 1);
  }
  50% {
    transform: translateY(0);
    animation-timing-function: cubic-bezier(0, 0, 0.2, 1);
  }
}
</style>
