<script setup>
import { onMounted, ref } from 'vue'
import { promiseTimeout, useTimeout } from '@vueuse/core'
import { useFeedbackStore } from '@/stores/feedback.js'
import { useHelpdeskStore } from '@/stores/helpdesk.js'

const isComplete = ref(false)
const message = ref('Sending')
const helpdeskStore = useHelpdeskStore()

const ready = useTimeout(1000)

onMounted(async () => {
  await promiseTimeout(1800)
  if (!helpdeskStore.windowOption.isError && ready.value) {
    isComplete.value = true
    message.value = 'Success'
  }
})
</script>

<template>
  <div class="absolute w-full h-full top-0 left-0 bg-gray-400 bg-opacity-55 flex">
    <div
      class="w-[85%] h-[50%] bg-white rounded-bl-3xl rounded-tr-3xl m-auto flex flex-col justify-center items-center gap-3"
    >
      <svg
        xmlns="http://www.w3.org/2000/svg"
        class="animate-spin"
        height="72"
        viewBox="0 -960 960 960"
        width="72"
        v-if="!isComplete"
      >
        <path
          d="M160-160v-80h110l-16-14q-52-46-73-105t-21-119q0-111 66.5-197.5T400-790v84q-72 26-116 88.5T240-478q0 45 17 87.5t53 78.5l10 10v-98h80v240H160Zm400-10v-84q72-26 116-88.5T720-482q0-45-17-87.5T650-648l-10-10v98h-80v-240h240v80H690l16 14q49 49 71.5 106.5T800-482q0 111-66.5 197.5T560-170Z"
        />
      </svg>

      <svg
        xmlns="http://www.w3.org/2000/svg"
        height="72"
        viewBox="0 -960 960 960"
        width="72"
        v-else
      >
        <path d="M382-240 154-468l57-57 171 171 367-367 57 57-424 424Z" />
      </svg>
      <h1 class="text-xl">{{ message }}</h1>
    </div>
  </div>
</template>

<style scoped></style>