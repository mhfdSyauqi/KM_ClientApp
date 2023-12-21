<script setup>
import appIcon from '@/assets/appIcon.jpg'
</script>

<template>
  <div class="h-full bg-primary px-7 flex justify-start justify-items-center items-center">
    <div class="basis-[12%]">
      <img class="w-[60%] h-[60%] rounded-full" :src="appIcon" />
    </div>
    <h1 class="basis-[83%] font-semibold text-white">{{ 'Virtual ICT Helpdesk' }}</h1>
  </div>
</template>

<style scoped></style>
