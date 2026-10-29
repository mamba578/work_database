export function usePhoneMask() {
  function formatPhone(value: string): string {
    let digits = value.replace(/\D/g, '')
    if (digits.startsWith('7')) digits = digits.substring(1)
    if (digits.length > 10) digits = digits.substring(0, 10)

    let f = '+7 '
    if (digits.length >= 1) f += '(' + digits.substring(0, Math.min(3, digits.length))
    if (digits.length >= 4) f += ') ' + digits.substring(3, Math.min(3, digits.length - 3) + 3)
    if (digits.length >= 7) f += ' - ' + digits.substring(6, Math.min(2, digits.length - 6) + 6)
    if (digits.length >= 9) f += ' - ' + digits.substring(8, Math.min(2, digits.length - 8) + 8)
    return f
  }

  return { formatPhone }
}