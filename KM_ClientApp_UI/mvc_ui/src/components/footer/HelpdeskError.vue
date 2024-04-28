<script setup>
import IconWarning from '@/components/icons/IconWarning.vue'
import { useHelpdeskStore } from '@/stores/helpdesk.js'

const helpdeskStore = useHelpdeskStore()

function onClickResend() {
  helpdeskStore.windowOption.isError = false
  helpdeskStore.windowOption.isOpen = true
  helpdeskStore.windowOption.isSending = false
}
</script>

<template>
  <div class="absolute w-full h-full top-0 left-0 bg-gray-400 bg-opacity-55 flex">
    <div
      class="w-[90%] h-[70%] bg-white rounded-bl-3xl rounded-tr-3xl m-auto flex flex-col justify-center items-center gap-3"
    >
      <IconWarning />

      <h1 class="text-xl text-center">
        Ops Something when wrong, please
        <button
          class="rounded-full px-2 bg-primary drop-shadow-md text-white cursor-pointer active:scale-95"
          @click.prevent="onClickResend"
        >
          Resend
        </button>
        your message
      </h1>
    </div>
  </div>
</template>

<style scoped></style>