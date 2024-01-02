<script setup>
import appIcon from '@/assets/appIcon.jpg'
import { useConfigStore } from '@/stores/config'
import IconRefresh from '../icons/IconRefresh.vue'

const configStore = useConfigStore()
const botMessageText = 'Mohon maaf sepertinya terjadi kesalahan pada sistem...'

function LoadAlterNative(e) {
  e.target.src = appIcon
}

function RefreshWindow() {
  window.location.reload()
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
      class="basis-auto max-w-[50%] h-full text-sm bg-red-600 text-white drop-shadow-lg p-3 rounded-xl rounded-bl-none"
    >
      <p class="break-words">{{ botMessageText }}</p>

      <p class="text-right text-xs text-gray-500">
        <button
          class="bg-gray-600 border-2 border-gray-300 py-1 px-1.5 rounded-full text-sm hover:bg-white drop-shadow-md hover:border-2 hover:border-gray-600 cursor-pointer active:scale-90"
          @click.prevent="RefreshWindow"
        >
          <IconRefresh class="fill-white hover:fill-gray-600" />
        </button>
      </p>
    </div>
  </div>
</template>

<style scoped></style>
