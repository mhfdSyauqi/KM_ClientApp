<script setup></script>

<template>
  <div
    class="fixed w-screen h-screen lg:h-4/5 lg:w-1/3 right-0 bottom-0 bg-gray-100 flex flex-col flex-nowrap drop-shadow-sm font-poppins"
  >
    <div class="basis-[10%] max-h-[10%]">
      <slot name="header"></slot>
    </div>

    <div class="basis-[78%] max-h-[78%]">
      <slot name="body"></slot>
    </div>

    <div class="basis-[12%]">
      <slot name="footer"></slot>
    </div>
  </div>
</template>

<style scoped></style>
