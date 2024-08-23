<script setup>
import ButtonCategory from '@/components/body/ButtonCategory.vue'
import BotCategories from '@/shared/botCategories'

import { useContentStore } from '@/stores/content'
import { useHelpdeskStore } from '@/stores/helpdesk.js'
import IconUndo from '@/components/icons/IconUndo.vue'

const contentStore = useContentStore()
const helpdeskStore = useHelpdeskStore()
const props = defineProps({
  record: {
    type: BotCategories,
    required: true
  }
})

const isCommon = props.record.is_common
const createAt = props.record.time
const currSearchedId = props.record.searched_identity
const currSearchedKeyword = props.record.searched_keyword
const currLayer = props.record.layer
const currPage = props.record.paginations?.current
const nextLayer = props.record.layer + 1
const nextPage = props.record.paginations?.next
const prevPage = props.record.paginations?.previous

async function SelectedCategory(categoryObj) {
  if (!isCommon) {
    return await contentStore.Searched.SelectedCategory(categoryObj, createAt)
  }
  return await contentStore.Common.SelectedCategory(categoryObj, nextLayer, createAt)
}

async function LoadMore() {
  if (!isCommon) {
    return await contentStore.Searched.LoadMoreCategory(currSearchedKeyword, nextPage, createAt)
  }
  return await contentStore.Common.LoadMoreCategory(currSearchedId, nextPage, createAt)
}

async function GoBack() {
  if (!isCommon) {
    return await contentStore.Searched.GoBackCategory(currSearchedKeyword, prevPage, createAt)
  }

  return await contentStore.Common.GoBackCategory(currSearchedId, currLayer, prevPage, createAt)
}

async function GoMainMenu() {
  if (!isCommon) {
    return await contentStore.Searched.GoMainMenu(createAt)
  }
  return await contentStore.Common.GoMainMenu(currLayer, createAt)
}
</script>

<template>
  <template
    v-if="
      !props.record.is_closed &&
      !props.record.is_reasked &&
      !props.record.is_idle &&
      !props.record.selected
    "
  >
    <div class="flex justify-start items-end">
      <div class="basis-[12%]">&nbsp;</div>
      <ul class="basis-[70%] flex flex-row flex-wrap items-start gap-2 mr-7">
        <li v-if="currLayer === 3 && currPage === 1">
          <ButtonCategory title="Previous Layer" @click.prevent="GoBack">
            <IconUndo class="fill-gray-500 h-5 w-8" />
          </ButtonCategory>
        </li>

        <li
          v-for="category in props.record.items"
          :key="category.id"
          @click="SelectedCategory(category)"
        >
          <ButtonCategory>
            {{ category.name }}
          </ButtonCategory>
        </li>

        <li v-if="props.record.is_not_found">
          <ButtonCategory @click.prevent="helpdeskStore.openWindow"
            >Send Email to Helpdesk</ButtonCategory
          >
        </li>

        <li v-if="nextPage !== null && !props.record.is_not_found">
          <ButtonCategory @click.prevent="LoadMore">More...</ButtonCategory>
        </li>
        <li v-if="currPage > 1">
          <ButtonCategory @click.prevent="GoBack">Back</ButtonCategory>
        </li>
        <li v-if="currLayer > 1 || !isCommon">
          <ButtonCategory @click.prevent="GoMainMenu">Main Menu</ButtonCategory>
        </li>
      </ul>
    </div>
  </template>

  <template v-if="props.record.is_closed || (props.record.is_idle && !props.record.selected)">
    <div class="flex justify-start items-end">
      <div class="basis-[12%]">&nbsp;</div>
      <ul class="basis-[70%] flex flex-row flex-wrap items-start gap-2 mr-7">
        <li>
          <ButtonCategory @click.prevent="contentStore.SuggestedCategoryContent('Yes', 1)">
            Yes
          </ButtonCategory>
        </li>
        <li>
          <ButtonCategory @click.prevent="contentStore.EndConversationContent()">
            No
          </ButtonCategory>
        </li>
      </ul>
    </div>
  </template>

  <template v-if="props.record.is_reasked && !props.record.selected">
    <div class="flex justify-start items-end">
      <div class="basis-[12%]">&nbsp;</div>
      <ul class="basis-[70%] flex flex-row flex-wrap items-start gap-2 mr-7">
        <li>
          <ButtonCategory @click.prevent="contentStore.ReAskedContent(currSearchedId)">
            Yes
          </ButtonCategory>
        </li>
        <li>
          <ButtonCategory @click.prevent="contentStore.EndingReAskedContent()"> No </ButtonCategory>
        </li>
      </ul>
    </div>
  </template>
</template>

<style scoped></style>