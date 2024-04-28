<script setup>
import HelpdeskInput from '@/components/footer/HelpdeskInput.vue'
import HelpdeskSending from '@/components/footer/HelpdeskSending.vue'
import HelpdeskError from '@/components/footer/HelpdeskError.vue'
import { useHelpdeskStore } from '@/stores/helpdesk.js'

const helpdeskStore = useHelpdeskStore()
</script>

<template>
  <HelpdeskInput v-if="helpdeskStore.windowOption.isOpen" />
  <HelpdeskSending v-if="helpdeskStore.windowOption.isSending" />
  <HelpdeskError v-if="helpdeskStore.windowOption.isError" />
</template>

<style scoped></style>