import {
  Drop,
  Gauge,
  Plant,
  Question,
  ThermometerSimple,
  type Icon,
  type IconProps,
} from '@phosphor-icons/react'

const ICONS: Record<string, Icon> = {
  temperature: ThermometerSimple,
  humidity: Drop,
  soil: Plant,
  pressure: Gauge,
}

export function MeasurementIcon({ kind, ...props }: { kind: string } & IconProps) {
  const IconComponent = ICONS[kind] ?? Question
  return <IconComponent {...props} />
}