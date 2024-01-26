<script setup>
import IconStar from '@/components/icons/IconStar.vue'
import { useFeedbackStore } from '@/stores/feedback.js'
import { ref } from 'vue'

const feedbackStore = useFeedbackStore()
const remarkEl = ref(null)

function onMouseHover(rating) {
  const maxKey = Math.max(...feedbackStore.ratingStars.map((prop) => prop.key))
  if (rating.key > 1) {
    feedbackStore.ratingStars.map((prop) => (prop.key < rating.key ? (prop.isHover = true) : null))
  }

  if (rating.key < maxKey) {
    feedbackStore.ratingStars.map((prop) => (prop.key > rating.key ? (prop.isHover = false) : null))
  }
  rating.isHover = true
}

function onMoseLeave() {
  if (feedbackStore.rating.value > 0) {
    return feedbackStore.ratingStars.map((prop) =>
      prop.key <= feedbackStore.rating.value ? (prop.isHover = true) : (prop.isHover = false)
    )
  }
  feedbackStore.ratingStars.map((prop) => (prop.isHover = false))
}

function onRated(rating) {
  if (feedbackStore.rating.value === rating.key) {
    feedbackStore.rating.value = 0
    feedbackStore.ratingStars.map((prop) => (prop.isHover = false))
    return
  }
  feedbackStore.rating.value = rating.key
  feedbackStore.ratingStars.map((prop) => (prop.key <= rating.key ? (prop.isHover = true) : null))
  remarkEl.value.focus()
}
</script>

<template>
  <div class="absolute w-full h-full top-0 left-0 bg-gray-400 bg-opacity-55 flex">
    <div
      class="w-[90%] h-[70%] bg-white rounded-bl-3xl rounded-tr-3xl m-auto flex flex-col px-10 py-7 justify-between"
    >
      <h1 class="font-bold xl:text-2xl">How satisfied are you with our service ?</h1>

      <div class="rating-wrapper">
        <p class="text-gray-600 text-sm xl:text-base">Add your rating and feedback message</p>
        <div class="flex" @mouseleave="onMoseLeave">
          <template v-for="n in feedbackStore.ratingStars" :key="n.key">
            <IconStar
              :is-hover="n.isHover"
              class="w-8 h-8 xl::w-10 xl:h-10 fill-amber-400 cursor-pointer active:scale-95"
              @mouseover.prevent="onMouseHover(n)"
              @click.prevent="onRated(n)"
            />
          </template>
          <p class="text-sm xl:text-base">
            <small
              class="mt-2 text-red-600 transition duration-150 ease-in"
              v-show="feedbackStore.rating.showErr"
            >
              {{ feedbackStore.rating.errMsg }}
            </small>
          </p>
        </div>
      </div>

      <div class="remark-wrapper">
        <h4 class="font-bold text-gray-600 mb-2 text-sm xl:text-base">FEEDBACK</h4>
        <p class="text-gray-600 mb-1 text-sm xl:text-base">
          What can we improve ?
          <small class="text-red-600" v-show="feedbackStore.remark.showErr">
            {{ feedbackStore.remark.errMsg }}
          </small>
        </p>
        <textarea
          class="w-full rounded-lg py-2 px-3 border-2 resize-none focus:outline-none focus:border-lime-300"
          ref="remarkEl"
          maxlength="280"
          v-model="feedbackStore.remark.value"
        />
      </div>

      <div class="flex justify-end gap-3 lg:text-sm xl:text-base">
        <button
          class="w-1/5 bg-transparent py-2 text-red-700 font-medium rounded-xl hover:bg-red-700 hover:text-white active:scale-95 max-sm:w-1/2 max-sm:bg-red-700 max-sm:text-white text-center"
          @click="feedbackStore.windowOption.isOpen = false"
        >
          Cancel
        </button>
        <button
          class="w-1/5 bg-primary py-2 text-white hover:bg-green-800 rounded-xl drop-shadow-md active:scale-95 max-sm:w-1/2"
          @click.prevent="feedbackStore.SendFeedback"
        >
          Send
        </button>
      </div>
    </div>
  </div>
</template>
