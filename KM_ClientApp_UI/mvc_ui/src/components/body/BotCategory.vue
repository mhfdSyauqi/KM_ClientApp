<script setup>
import ButtonCategory from '@/components/body/ButtonCategory.vue'
import BotCategories from '@/shared/botCategories'

import { useCategoryStore } from '@/stores/category'

const categoryStore = useCategoryStore()
const props = defineProps({
  record: {
    type: BotCategories,
    required: true
  }
})
const currLayer = props.record.layer
const currPage = props.record.paginations.current
const nextPage = props.record.paginations.next
const prevPage = props.record.paginations.previous

function MoreCategory() {
  // DO SOME THING
}

function BackToCategory() {}

function BackToMainMenu() {}
</script>

<template>
  <div class="flex justify-start items-end" v-if="!props.record.selected">
    <div class="basis-[12%]">&nbsp;</div>
    <ul class="basis-[70%] flex flex-row flex-wrap items-start gap-2 mr-7">
      <li
        v-for="category in props.record.items"
        :key="category.id"
        @click="categoryStore.SelectedCategory(category, props.record.time)"
      >
        <ButtonCategory>
          {{ category.name }}
        </ButtonCategory>
      </li>

      <li v-if="nextPage !== null || props.record.items.length > 0">
        <ButtonCategory @click.prevent="MoreCategory()">More...</ButtonCategory>
      </li>
      <li v-if="currLayer >= 2 || currPage > 1">
        <ButtonCategory @click.prevent="BackToCategory()">Go Back</ButtonCategory>
      </li>
      <li v-if="currLayer > 1">
        <ButtonCategory @click.prevent="BackToMainMenu()">Go Main Menu</ButtonCategory>
      </li>
    </ul>
  </div>
</template>

<style scoped></style>
