<script setup>
import IconSend from '@/components/icons/IconSend.vue'
import { useContentStore } from '@/stores/content'
import { promiseTimeout, useTimeout } from '@vueuse/core'
import { ref, watchEffect } from 'vue'
import { useConfigStore } from '@/stores/config.js'

const wordsLimit = 100
const contentStore = useContentStore()
const configStore = useConfigStore()

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
  const keywordLimit = configStore.appConfig.keywords
  if (searchedLength >= 0 && searchedLength < keywordLimit) {
    ToggleShowError('on')
    SetUserInformation(`Minimal keyword tediri dari ${keywordLimit} huruf`)
    return
  }

  ToggleShowError('off')
  ToggleShowInformation('off')
  const searchKeyword = searchInput.value
  searchInput.value = ''
  userInformation.value = ''
  await contentStore.SearchedCategoryContent(searchKeyword)
}
</script>

<template>
  <div class="w-full h-full flex">
    <div
      class="basis-full my-auto mx-5 lg:mx-9 rounded-lg flex drop-shadow-md group bg-transparent"
    >
      <p
        ref="infoEl"
        class="absolute -top-3.5 max-lg:-top-5 left-1 text-xs max-lg:text-sm"
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
        @keydown.enter.prevent="UserSubmit"
        class="w-[85%] rounded-l-lg p-1 md:p-1.5 max-lg:py-2 max-lg:px-3 border-2 border-r-0 resize-none focus:outline-none focus:border-lime-300 placeholder:italic placeholder:text-sm"
        placeholder="Insert keyword....."
        rows="1"
      ></textarea>
      <button
        @click="UserSubmit"
        class="w-[15%] bg-primary rounded-r-md flex justify-center items-center drop-shadow-md"
      >
        <IconSend class="fill-white" />
      </button>
    </div>
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