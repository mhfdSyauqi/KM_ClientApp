<script setup>
import appIcon from '@/assets/appIcon.jpg'
import BotResponseContent from '@/shared/botContent.js'

import { useConfigStore } from '@/stores/config'
import { useDateFormat } from '@vueuse/core'

const configStore = useConfigStore()
const props = defineProps({
  record: {
    type: BotResponseContent,
    required: true
  }
})

const botSendAt = useDateFormat(new Date(), 'HH:mm')
const botContent = props.record.description
const botLink = props.record.id
const botMessages = props.record.messages

function LoadAlterNative(e) {
  e.target.src = appIcon
}
</script>

<template>
  <div class="flex justify-start items-end">
    <div class="basis-[12%]">
      <img
        class="w-[70%] h-[70%] rounded-full"
        @error.prevent="LoadAlterNative"
        :src="configStore.appConfig?.app_image"
        alt="appIcon"
      />
    </div>
    <div
      class="basis-auto max-w-[50%] h-full text-sm bg-white drop-shadow-lg p-3 rounded-xl rounded-bl-none"
    >
      <div v-show="botMessages?.length > 0">
        <p v-for="(message, index) in botMessages" :key="index">
          {{ message.text }}
        </p>
        <br />
      </div>

      <div class="bot-content" v-html="botContent"></div>

      <div>
        <br />
        <small>todo : update artile link</small>
        <br />
        <a class="text-primary visited:text-gray-500" :href="botLink" target="_blank"
          ><i>Read More...</i></a
        >
      </div>

      <p class="text-right text-xs text-gray-500">{{ botSendAt }}</p>
    </div>
  </div>
</template>

<style scoped>
.bot-content :deep(ol, ul) {
  @apply list-inside list-decimal;
}

.bot-content :deep(p) {
  @apply text-left break-words;
}
</style>
