<script setup>
import { onUpdated, ref } from 'vue'

const scrollPosition = ref(0)

onUpdated(async () => {
  scrollPosition.value.scrollIntoView({ behavior: 'smooth' })
})
</script>

<template>
  <div class="h-full bg-gray-100 p-5 gap-3 overflow-y-auto flex flex-col overflow-x-hidden">
    <p class="text-center text-xs text-gray-500">Today</p>

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
