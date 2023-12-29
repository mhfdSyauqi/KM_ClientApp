<script setup>
import appIcon from '@/assets/appIcon.jpg'

import BotRecord from '@/shared/botRecord'

import { useConfigStore } from '@/stores/config'
import { useDateFormat } from '@vueuse/core'

const configStore = useConfigStore()
const props = defineProps({
  record: {
    type: BotRecord,
    required: true
  }
})

const botSendAt = useDateFormat(props.record.time, 'HH:mm')
const botMessageText = props.record.message.text
const botMessageType = props.record.message.type
</script>

<template>
  <div class="flex justify-start items-end">
    <div class="basis-[12%]">
      <img
        class="w-[70%] h-[70%] rounded-full"
        :src="configStore.appConfig?.app_image ?? appIcon"
        alt="bg-app-logo"
        v-show="botMessageType === 'desc'"
      />
    </div>
    <div
      class="basis-auto max-w-[50%] h-full text-sm bg-white drop-shadow-lg p-3 rounded-xl rounded-bl-none"
    >
      <p class="break-words">{{ botMessageText }}</p>

      <p class="text-right text-xs text-gray-500">{{ botSendAt }}</p>
    </div>
  </div>
</template>

<style scoped></style>
