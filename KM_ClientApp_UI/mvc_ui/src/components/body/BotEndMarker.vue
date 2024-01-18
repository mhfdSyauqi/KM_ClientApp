<script setup>
import { useConfigStore } from '@/stores/config.js'

const configStore = useConfigStore()
</script>

<template>
  <div class="flex justify-start items-end mt-3">
    <div class="basis-[12%]">&nbsp;</div>
    <div class="basis-[70%] text-gray-600 text-sm">
      <p class="text-wrap">
        {{ configStore.appConfig?.app_name ?? 'ChatBot' }} has marked the conversation as finish
      </p>
    </div>
  </div>
</template>

<style scoped></style>