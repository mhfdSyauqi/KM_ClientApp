<script setup>
import BotCategory from '@/components/body/BotCategory.vue'
import BotError from '@/components/body/BotError.vue'
import BotMessage from '@/components/body/BotMessage.vue'
import BotTyping from '@/components/body/BotTyping.vue'
import UserMessage from '@/components/body/UserMessage.vue'

import BotCategories from '@/shared/botCategories'
import BotRecord from '@/shared/botRecord'
import UserRecord from '@/shared/userRecord'

import { useContentStore } from '@/stores/content'
import { useSessionStore } from '@/stores/session'

import { onMounted, onUpdated, ref } from 'vue'

const sessionStore = useSessionStore()
const contentStore = useContentStore()
const scrollPosition = ref(0)

onMounted(async () => {
  await sessionStore.sessionHandler.get()
  if (sessionStore.userSession.records.length === 0) {
    await contentStore.StartUpContent()
  }
})

onUpdated(async () => {
  scrollPosition.value.scrollIntoView({ behavior: 'smooth' })
})
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

      <template v-if="item.actor === 'error'">
        <BotTyping v-if="!item.rendered" />
        <BotError v-else />
      </template>
    </template>

    <span ref="scrollPosition">&nbsp;</span>
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
