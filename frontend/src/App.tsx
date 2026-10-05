import { ModuleCard } from './components/ModuleCard'
import type { Module } from './types'
import { useModules } from './useModlues'


export default function App() {
  const { modules, error } = useModules()

  return (
    <main className="mx-auto flex min-h-screen max-w-[1480px] flex-col gap-6 px-4 pb-16 pt-6 sm:px-10">
      <header className="flex flex-wrap items-end justify-between gap-4 pb-3 pt-6">
        <div className="flex flex-col gap-2">
          <h1 className="m-0 text-3xl font-normal leading-none tracking-[-0.07em]">IOTHOME</h1>
          <div className="flex flex-wrap items-center gap-3 text-[13px] tabular-nums text-neutral-500">
            <span className="flex items-center gap-1.5 text-accent-300">
              <span className="h-1.5 w-1.5 rounded-full bg-accent shadow-[0_0_8px_var(--color-accent)]" />
              online
            </span>
            <span>
              modules · measurements
            </span>
            {/* {low > 0 && (
              <span className="flex items-center gap-1.5 text-warn">
                <BatteryWarning size={15} /> {low} low battery
              </span>
            )} */}
          </div>
        </div>
{/*         <SegmentedControl label="Sort modules" options={SORTS} value={sort} onChange={setSort} />
 */}      </header>

      <section className="grid grid-cols-[repeat(auto-fill,minmax(310px,1fr))] items-stretch gap-4">
        {modules?.map(m => (
          <ModuleCard key={m.id} module={m}/>
        ))}
      </section>

      {/* {open && (
        <div role="dialog" aria-modal="true" aria-label={open.name} onClick={() => setOpenId(null)} className="fixed inset-0 z-50 overflow-auto bg-black/60 p-3 backdrop-blur-sm">
          <div onClick={(e) => e.stopPropagation()} className="mx-auto my-10 w-full max-w-[720px]">
            <ModuleDetail module={open} now={now} onClose={() => setOpenId(null)} />
          </div>
        </div>
      )} */}
    </main>
  )
}