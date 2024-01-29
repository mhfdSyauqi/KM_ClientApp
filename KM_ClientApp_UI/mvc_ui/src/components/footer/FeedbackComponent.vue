<script setup>
import FeedbackError from '@/components/footer/FeedbackError.vue'
import FeedbackInput from '@/components/footer/FeedbackInput.vue'
import FeedbackSending from '@/components/footer/FeedbackSending.vue'
import { useFeedbackStore } from '@/stores/feedback.js'

const feedbackStore = useFeedbackStore()
</script>

<template>
  <div class="w-full h-full flex">
    <div class="basis-full my-auto mx-5 drop-shadow-md bg-transparent">
      <button
        class="w-full bg-primary hover:bg-green-800 rounded-xl p-2.5 drop-shadow-md text-white cursor-pointer active:scale-95"
        @click="feedbackStore.windowOption.isOpen = true"
      >
        Rate Conversation
      </button>
    </div>
  </div>

  <section id="feedback-modal">
    <FeedbackInput v-if="feedbackStore.windowOption.isOpen" />
    <FeedbackSending v-if="feedbackStore.windowOption.isSending" />
    <FeedbackError v-if="feedbackStore.windowOption.isError" />
  </section>
</template>