<script setup>
import ChatbotBubble from '@components/ChatbotBubble.vue'
import ChatbotFramer from '@components/ChatbotFramer.vue'

import { useConfigStore } from '@stores/config'

import { onBeforeMount } from 'vue'

const configStore = useConfigStore()

onBeforeMount(async () => {
  await configStore.InitAppConfig()
})
</script>

<template>
  <div class="select-none">
    <ChatbotBubble v-if="!configStore.windowInfo.isOpen" />
    <ChatbotFramer v-else />
  </div>
</template>

<style scoped></style>
