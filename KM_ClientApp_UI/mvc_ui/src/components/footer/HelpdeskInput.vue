<script setup>
import { useHelpdeskStore } from '@/stores/helpdesk.js'

const helpdeskStore = useHelpdeskStore()

const textAreaRow = window.innerHeight >= 766 && window.innerWidth > 442 ? 4 : null
</script>

<template>
  <div class="absolute w-full h-full top-0 left-0 bg-gray-400 bg-opacity-55 flex px-5 2xl:px-0">
    <div
      class="w-[95%] h-[50%] md:w-[75%] md:h-[40%] lg:w-[85%] lg:h-[50%] 2xl:w-[80%] 2xl:h-[40%] bg-white rounded-bl-3xl rounded-tr-3xl m-auto flex flex-col px-10 py-5 md:py-7 gap-3.5"
    >
      <h1 class="basis-[25%] font-bold text-xl xl:text-2xl text-center">Send us a message!</h1>

      <div class="basis-[60%] remark-wrapper">
        <p class="text-gray-600 mb-1 text-sm xl:text-base">
          Message
          <small class="text-red-600">* {{ helpdeskStore.userMessage.errMsg }} </small>
        </p>
        <textarea
          class="w-full rounded-lg py-2 px-3 border-2 resize-none focus:outline-none focus:border-lime-300"
          ref="remarkEl"
          :rows="textAreaRow"
          maxlength="280"
          v-model="helpdeskStore.userMessage.content"
        />
      </div>

      <div class="basis-[15%] lg:text-sm xl:text-base flex justify-end gap-3">
        <button
          class="w-auto max-md:w-1/2 bg-transparent p-2 max-md:py-1.5 text-red-700 font-medium rounded-xl hover:bg-red-700 hover:text-white active:scale-95 max-sm:w-1/2 max-sm:bg-red-700 max-sm:text-white text-center"
          @click="helpdeskStore.closeWindow"
        >
          Cancel
        </button>
        <button
          class="w-auto max-md:w-1/2 bg-primary p-2 max-md:py-1.5 text-white hover:bg-green-800 rounded-xl drop-shadow-md active:scale-95 max-sm:w-1/2"
          @click.prevent="helpdeskStore.SendMailHelpdesk"
        >
          Send
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped></style>