<script setup>
import FeedbackComponent from '@/components/footer/FeedbackComponent.vue'
import SearchComponent from '@/components/footer/SearchComponent.vue'
import StartComponent from '@/components/footer/StartComponent.vue'
import { useSessionStore } from '@/stores/session'

const sessionStore = useSessionStore()
</script>

<template>
  <SearchComponent
    v-if="sessionStore.userSession.is_active && !sessionStore.userSession.has_feedback"
  />

  <FeedbackComponent
    v-if="!sessionStore.userSession.is_active && !sessionStore.userSession.has_feedback"
  />

  <StartComponent
    v-if="!sessionStore.userSession.is_active && sessionStore.userSession.has_feedback"
  />
</template>

<style scoped></style>
