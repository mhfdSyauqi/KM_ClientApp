<script setup>
import FeedbackError from '@/components/footer/FeedbackError.vue'
import FeedbackInput from '@/components/footer/FeedbackInput.vue'
import FeedbackSending from '@/components/footer/FeedbackSending.vue'
import { useFeedbackStore } from '@/stores/feedback.js'

const feedbackStore = useFeedbackStore()
</script>

<template>
  <div class="mx-5 mt-4 lg:mx-9 drop-shadow-md bg-transparent">
    <button
      class="w-full bg-primary hover:bg-green-800 rounded-xl p-1.5 xl:p-3 drop-shadow-md text-white cursor-pointer active:scale-95"
      @click="feedbackStore.windowOption.isOpen = true"
    >
      Rate Conversation
    </button>
  </div>
  <section id="feedback-modal">
    <FeedbackInput v-if="feedbackStore.windowOption.isOpen" />
    <FeedbackSending v-if="feedbackStore.windowOption.isSending" />
    <FeedbackError v-if="feedbackStore.windowOption.isError" />
  </section>
</template>
