<script setup>
import IconSend from '@/components/icons/IconSend.vue'
</script>

<template>
  <div class="ml-9 mt-3 rounded-lg flex drop-shadow-md group bg-transparent">
    <input
      class="w-[85%] rounded-l-lg py-2 px-3 border-2 focus:outline-none focus:border-lime-300 placeholder:italic placeholder:text-sm"
      type="text"
      placeholder="Insert keyword....."
      ref="search"
    />
    <button
      class="w-[10%] bg-primary rounded-md -ml-3 flex justify-center items-center drop-shadow-md"
    >
      <IconSend class="fill-white" />
    </button>
  </div>
</template>

<style scoped></style>
