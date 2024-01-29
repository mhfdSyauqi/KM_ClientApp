<script setup>
async function StartNewSession() {
  return window.location.reload()
}
</script>

<template>
  <div class="w-full h-full flex">
    <div class="basis-full my-auto mx-5 drop-shadow-md bg-transparent">
      <button
        class="w-full bg-primary hover:bg-green-800 rounded-xl p-2.5 drop-shadow-md text-white cursor-pointer active:scale-95"
        @click.prevent="StartNewSession"
      >
        Start New Conversation
      </button>
    </div>
  </div>
</template>

<style scoped></style>