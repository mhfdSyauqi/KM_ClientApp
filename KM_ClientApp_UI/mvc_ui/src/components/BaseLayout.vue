<script setup></script>

<template>
  <div
    class="fixed w-screen h-screen lg:h-4/5 lg:w-1/3 right-0 bottom-0 bg-gray-100 flex flex-col flex-nowrap drop-shadow-sm font-poppins select-none"
  >
    <div class="basis-[11%] max-h-[11%] xl:basis-[10%] xl:max-h-[10%]">
      <slot name="header"></slot>
    </div>

    <div class="basis-[75%] max-h-[75%] xl:basis-[78%] xl:max-h-[78%]">
      <slot name="body"></slot>
    </div>

    <div class="basis-[14%] max-h-[14%] xl:basis-[12%] xl:max-h-[12%]">
      <slot name="footer"></slot>
    </div>
  </div>
</template>

<style scoped></style>
