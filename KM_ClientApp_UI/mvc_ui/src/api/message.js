import { useMyFetch } from '@/shared/useMyFetch'

export const MessageType = {
  closing: 'closing',
  feedback: 'feedback',
  idle: 'idle',
  layer_one: 'layers/1',
  layer_two: 'layers/2',
  layer_three: 'layers/3',
  mail_sended: 'mail_sended',
  not_found: 'not_found',
  reasked: 'reasked',
  searched: 'searched',
  solved: 'solved',
  suggestion: 'suggestion',
  welcome: 'welcome'
}

export async function GetMessageByTypeAsync(TYPE, SELECTED) {
  const PAYLOAD =
    SELECTED !== null
      ? {
          Selected_Category: SELECTED
        }
      : {}

  const { data, statusCode } = await useMyFetch(`message/${TYPE}`).post(PAYLOAD).json()

  return {
    is_success: statusCode.value === 200 ? true : false,
    messages: data.value?.data.messages
  }
}
