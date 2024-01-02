<script setup>
import UserRecord from '@/shared/userRecord'

import { useDateFormat } from '@vueuse/core'

const props = defineProps({
  record: {
    type: UserRecord,
    required: true
  }
})

const userSendAt = useDateFormat(props.record.time, 'HH:mm')
const userMessage = props.record.message
</script>

<template>
  <div class="flex justify-end items-end">
    <div
      class="basis-auto min-w-[17%] max-w-[50%] h-full text-sm bg-amber-500 drop-shadow-lg p-3 rounded-xl rounded-br-none"
    >
      <p class="break-words">{{ userMessage }}</p>

      <p class="text-right text-xs text-gray-700">{{ userSendAt }}</p>
    </div>
  </div>
</template>

<style scoped></style>
