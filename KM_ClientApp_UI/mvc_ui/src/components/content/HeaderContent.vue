<script setup>
import appIcon from '@/assets/appIcon.jpg'
import { useConfigStore } from '@/stores/config'

const configStore = useConfigStore()

function LoadAlterNative(e) {
  e.target.src = appIcon
}
</script>

<template>
  <div class="h-full bg-primary px-9 flex justify-start justify-items-center items-center">
    <div class="basis-[12%]">
      <img
        class="w-[60%] h-[60%] rounded-full"
        @error.prevent="LoadAlterNative"
        :src="configStore.appConfig?.app_image"
      />
    </div>
    <h1 class="basis-[83%] font-semibold text-white">
      {{ configStore.appConfig?.app_name }}
    </h1>
  </div>
</template>

<style scoped></style>
