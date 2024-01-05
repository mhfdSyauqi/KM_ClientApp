<script setup>
import IconSend from '@/components/icons/IconSend.vue'
import { useContentStore } from '@/stores/content'
import { promiseTimeout, useTimeout } from '@vueuse/core'
import { ref, watchEffect } from 'vue'

const wordsLimit = 100
const contentStore = useContentStore()

const userInformation = ref('')
const searchEl = ref(null)
const searchInput = ref('')
const showInformation = ref(false)
const showError = ref(false)

watchEffect(async () => {
  if (contentStore.isFocused) {
    SetUserInformation('Silahakan gunakan pencarian keyword')
    ToggleShowInformation('on')
    searchEl.value.focus()
    const ready = useTimeout(1000)
    await promiseTimeout(1000)
    if (ready.value) {
      contentStore.isFocused = !contentStore.isFocused
      ToggleShowInformation('off')
    }
  }

  if (contentStore.isSystemErr) {
    searchEl.value.disabled = true
  }
})

function SetUserInformation(message) {
  userInformation.value = message
}

function ToggleShowInformation(arg = null) {
  switch (arg) {
    case 'on':
      return (showInformation.value = true)
    case 'off':
      return (showInformation.value = false)
    default:
      return (showInformation.value = !showInformation.value)
  }
}

function ToggleShowError(arg = null) {
  switch (arg) {
    case 'on':
      return (showError.value = true)
    case 'off':
      return (showError.value = false)
    default:
      return (showError.value = !showInformation.value)
  }
}

function CheckUserInput() {
  const searchedLength = +searchInput.value.length
  const inputLimit = `${searchedLength}/${wordsLimit}`
  if (searchedLength > wordsLimit) {
    ToggleShowError('on')
    return SetUserInformation('Maaf keyword melebihi batas maksimal')
  }
  ToggleShowError('off')
  ToggleShowInformation('on')
  SetUserInformation(inputLimit)
}

async function UserSubmit() {
  if (showError.value) return

  const searchedLength = +searchInput.value.length
  if (searchedLength >= 0 && searchedLength < 3) {
    ToggleShowError('on')
    SetUserInformation('Minimal keyword tediri dari 3 huruf')
    return
  }

  ToggleShowError('off')
  ToggleShowInformation('off')
  return await contentStore.SearchedCategoryContent(searchInput.value).then(() => {
    userInformation.value = ''
    searchInput.value = ''
  })
}
</script>

<template>
  <div class="ml-9 mt-3 rounded-lg flex drop-shadow-md group bg-transparent">
    <p
      ref="infoEl"
      class="absolute -top-7 lg:-top-5 left-1 text-xs lg:text-sm"
      :class="[
        { hidden: !showInformation },
        { 'text-primary animate-bounce': contentStore.isFocused },
        { 'text-red-500 animate-bounce': showError }
      ]"
    >
      {{ userInformation }}
    </p>

    <textarea
      ref="searchEl"
      v-model="searchInput"
      @input="CheckUserInput"
      @keypress.enter="UserSubmit"
      class="w-[85%] rounded-l-lg py-2 px-3 border-2 border-r-0 resize-none focus:outline-none focus:border-lime-300 placeholder:italic placeholder:text-sm"
      placeholder="Insert keyword....."
      rows="1"
    ></textarea>
    <button
      @click="UserSubmit"
      class="w-[10%] bg-primary rounded-r-md flex justify-center items-center drop-shadow-md"
    >
      <IconSend class="fill-white" />
    </button>
  </div>
</template>

<style scoped>
::-webkit-scrollbar {
  width: 0.2rem;
}

/* Track */
::-webkit-scrollbar-track {
  box-shadow: inset 0 0 5px grey;
}

/* Handle */
::-webkit-scrollbar-thumb {
  @apply bg-lime-300;
}

/* Handle on hover */
::-webkit-scrollbar-thumb:hover {
  @apply bg-lime-300;
}
</style>
