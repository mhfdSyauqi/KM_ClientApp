import { useMyFetch } from '@/shared/useMyFetch'

export async function GetCategoriesAsync(SearchedId = null, PageNum = 1) {
  const PAYLOAD = JSON.stringify({
    Searched_Identity: SearchedId,
    Current_Page: PageNum
  })
  const { data, statusCode } = await useMyFetch(`category`).post(PAYLOAD).json()

  return {
    categories: data.value?.data.categories,
    is_success: statusCode.value === 200
  }
}

export async function GetReferenceCategoriesAsync(SearchedId) {
  const PAYLOAD = JSON.stringify({
    Id: SearchedId
  })
  const { data, statusCode } = await useMyFetch(`category/ref`).post(PAYLOAD).json()

  return {
    reference: data.value?.data.reference_categories.ref_id,
    is_success: statusCode.value === 200
  }
}

export async function PostHeatSelectedCategory(SessionId, HeatName, HeatId = null) {
  const PAYLOAD = JSON.stringify({
    Session_Id: SessionId,
    Heat_Name: HeatName,
    Heat_Id: HeatId
  })
  const { statusCode } = await useMyFetch(`category/heat`).post(PAYLOAD).json()

  return {
    is_success: statusCode.value === 204
  }
}

export async function SearchCategoriesAsync(searchedKeyword, nextPage = 1) {
  const PAYLOAD = JSON.stringify({
    Searched_Keyword: searchedKeyword,
    Current_Page: nextPage
  })
  const { statusCode, data } = await useMyFetch(`category/search`).post(PAYLOAD).json()

  return {
    categories: data.value?.data.searched_categories,
    is_success: statusCode.value === 200,
    is_not_found: statusCode.value === 404,
    is_single:
      data.value?.data.searched_categories.items.length === 1 &&
      data.value?.data.searched_categories.paginations.next === null &&
      data.value?.data.searched_categories.paginations.current === 1
  }
}

export async function SuggestCategoriesAsync(nextPage = 1) {
  const PAYLOAD = JSON.stringify({
    Current_Page: nextPage
  })
  const { statusCode, data } = await useMyFetch(`category/suggest`).post(PAYLOAD).json()

  return {
    is_success: statusCode.value === 200,
    categories: data.value?.data.suggested_categories
  }
}