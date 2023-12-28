<script setup>
import BotCategory from '@/components/body/BotCategory.vue'
import BotMessage from '@/components/body/BotMessage.vue'
import BotTyping from '@/components/body/BotTyping.vue'
import UserMessage from '@/components/body/UserMessage.vue'

import BotCategories from '@/shared/botCategories'
import BotRecord from '@/shared/botRecord'
import UserRecord from '@/shared/userRecord'

import { useConfigStore } from '@/stores/config'
import { useSessionStore } from '@/stores/session'
import { promiseTimeout, useTimeout } from '@vueuse/core'
import { onMounted, onUpdated, ref } from 'vue'

const sessionStore = useSessionStore()
const configStore = useConfigStore()
const scrollPosition = ref(0)
const delayTyping = configStore.appConfig?.delay_typing ?? 500

onMounted(async () => {
  await sessionStore.sessionHandler.get()
  if (sessionStore.userSession.records.length === 0) {
    await LoadStartup()
  }
})

onUpdated(async () => {
  scrollPosition.value.scrollIntoView({ behavior: 'smooth' })
})

async function LoadStartup() {
  const { GetMessageByTypeAsync, MessageType } = await import('@/api/message')
  const { is_success: msg_success, messages } = await GetMessageByTypeAsync(MessageType.welcome)

  if (!msg_success) {
    // DO SOME THING ERROR POP OUT
  }

  for (let i = 0; i <= messages.length - 1; i++) {
    const ready = useTimeout(delayTyping * i)
    const message = messages[i]
    sessionStore.recordHandler.addBotMessage('message', message)
    await promiseTimeout(delayTyping)
    if (ready.value) {
      sessionStore.recordHandler.markAsRendered()
    }
  }

  // const { GetCategoriesAsync } = await import('@/api/categories')
  // const { is_success: ctg_success, categories } = await GetCategoriesAsync()

  // if (!ctg_success) {
  //   // DO SOME THING ERROR POP OUT
  // }

  // sessionStore.recordHandler.addBotMessage('category', categories)

  return sessionStore.sessionHandler.update()
}
</script>

<template>
  <div class="h-full bg-gray-100 p-5 gap-3 overflow-y-auto flex flex-col overflow-x-hidden">
    <p class="text-center text-xs text-gray-500">Today</p>

    <template v-for="item in sessionStore.userSession.records" :key="item.time">
      <template v-if="item.actor === 'bot' && item.message">
        <BotTyping v-if="!item.rendered" />
        <BotMessage v-else :record="new BotRecord(item.time, item.message)" />
      </template>

      <template v-if="item.actor === 'bot' && item.categories">
        <BotCategory :record="new BotCategories(item.categories)" />
      </template>

      <template v-if="item.actor === 'user'">
        <UserMessage :record="new UserRecord(item.time, item.message.text)" />
      </template>
    </template>

    <span ref="scrollPosition"> &nbsp;</span>
  </div>
</template>

<style scoped>
::-webkit-scrollbar {
  width: 0.27rem;
}

/* Track */
::-webkit-scrollbar-track {
  box-shadow: inset 0 0 5px grey;
}

/* Handle */
::-webkit-scrollbar-thumb {
  background: #f59e0b;
}

/* Handle on hover */
::-webkit-scrollbar-thumb:hover {
  background: #f59e0b;
}
</style>
