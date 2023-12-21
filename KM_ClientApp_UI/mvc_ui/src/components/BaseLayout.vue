<script setup></script>

<template>
  <div
    class="fixed w-screen h-screen md:w-1/3 md:h-4/5 lg:w-1/3 right-0 bottom-0 bg-gray-100 flex flex-col flex-nowrap drop-shadow-sm font-poppins"
  >
    <div class="h-[9%] md:h-[10%]">
      <slot name="header"></slot>
    </div>

    <div class="h-[80%] md:h-[80%]">
      <slot name="body"></slot>
    </div>

    <div class="h-[12%] md:h[12%]">
      <slot name="footer"></slot>
    </div>
  </div>
</template>

<style scoped></style>
