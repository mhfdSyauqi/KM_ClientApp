<script setup>
async function StartNewSession() {
  return window.location.reload()
}
</script>

<template>
  <div class="mx-9 mt-8 md:mt-6 lg:mt-5 drop-shadow-md bg-transparent">
    <button
      class="w-full bg-primary hover:bg-green-800 rounded-xl p-3 drop-shadow-md text-white cursor-pointer active:scale-95"
      @click.prevent="StartNewSession"
    >
      Start New Conversation
    </button>
  </div>
</template>

<style scoped></style>