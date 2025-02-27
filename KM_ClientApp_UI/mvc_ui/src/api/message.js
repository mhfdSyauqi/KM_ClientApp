import { useMyFetch } from '@/shared/useMyFetch'

export const MessageType = Object.freeze({
  closing: 'closing',
  feedback: 'feedback',
  idle: 'idle',
  layer_one: 'layers/1',
  layer_two: 'layers/2',
  layer_three: 'layers/3',
  mail_sended: 'sended_mail',
  not_found: 'not_found',
  reasked: 'reasked',
  searched: 'searched',
  solved: 'solved',
  suggestion: 'suggestion',
  welcome: 'welcome'
})

export async function GetMessageByTypeAsync(TYPE, SELECTED) {
  const PAYLOAD = JSON.stringify(
    SELECTED !== null
      ? {
          Selected_Category: SELECTED
        }
      : {}
  )

  const { data, statusCode } = await useMyFetch(`message/${TYPE}`).post(PAYLOAD).json()

  return {
    messages: data.value?.data.messages,
    is_success: statusCode.value === 200
  }
}