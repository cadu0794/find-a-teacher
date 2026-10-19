
import { useState } from 'react'
import './App.css'

const DIAS = [
  { valor: 'segunda', label: 'Segunda-feira', curto: 'Seg' },
  { valor: 'terca', label: 'Terça-feira', curto: 'Ter' },
  { valor: 'quarta', label: 'Quarta-feira', curto: 'Qua' },
  { valor: 'quinta', label: 'Quinta-feira', curto: 'Qui' },
  { valor: 'sexta', label: 'Sexta-feira', curto: 'Sex' },
  { valor: 'sabado', label: 'Sábado', curto: 'Sáb' },
  { valor: 'domingo', label: 'Domingo', curto: 'Dom' },
]

type ProfessorElegivel = {
  id: number
  nome: string
  aulasNaSemana: number
  horarios: string[]
}

type ProfessorNaoElegivel = {
  id: number
  nome: string
  motivo: string
}

type BuscaResponse = {
  professores: ProfessorElegivel[]
  naoElegiveis: ProfessorNaoElegivel[]
}

type ErroResponse = {
  erros?: string[]
}

function App() {
  const [idioma, setIdioma] = useState('Inglês')
  const [dias, setDias] = useState<string[]>(['segunda', 'quarta'])
  const [inicioFaixa, setInicioFaixa] = useState('14:00')
  const [fimFaixa, setFimFaixa] = useState('18:00')
  const [duracaoMinutos, setDuracaoMinutos] = useState(60)

  const [resultado, setResultado] = useState<BuscaResponse | null>(null)
  const [carregando, setCarregando] = useState(false)
  const [erro, setErro] = useState('')

  function alternarDia(dia: string) {
    setDias((atuais) =>
      atuais.includes(dia)
        ? atuais.filter((item) => item !== dia)
        : [...atuais, dia],
    )
  }

  async function buscarProfessores(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setErro('')
    setResultado(null)

    if (dias.length === 0) {
      setErro('Selecione pelo menos um dia da semana.')
      return
    }

    if (fimFaixa <= inicioFaixa) {
      setErro('O horário final deve ser posterior ao inicial.')
      return
    }

    setCarregando(true)

    try {
      const resposta = await fetch('/api/professores/busca', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          idioma,
          dias,
          inicioFaixa,
          fimFaixa,
          duracaoMinutos,
        }),
      })

      const dados = await resposta.json()

      if (!resposta.ok) {
        const dadosErro = dados as ErroResponse
        throw new Error(
          dadosErro.erros?.join(' ') ||
            'Não foi possível realizar a busca. Confira os dados.',
        )
      }

      setResultado(dados as BuscaResponse)
    } catch (e) {
      setErro(
        e instanceof Error
          ? e.message
          : 'Não foi possível conectar à API. Verifique se o backend está rodando.',
      )
    } finally {
      setCarregando(false)
    }
  }

  return (
    <main className="app-shell">
      <header className="topbar">
        <a className="brand" href="/" aria-label="Find a Teacher - início">
          <span className="brand-mark">F</span>
          <span>find<span className="brand-light">a</span>teacher</span>
        </a>
        <span className="topbar-label">Plataforma de idiomas</span>
      </header>

      <section className="hero">
        <span className="eyebrow">
          <span className="status-dot" />
          ENCONTRE SEU PROFESSOR
        </span>
        <h1>
          Aprender um idioma
          <br />
          começa com <span>o professor certo.</span>
        </h1>
        <p>
          Encontre professores disponíveis de acordo com o idioma,
          seus dias e os horários que funcionam para você.
        </p>
      </section>

      <section className="search-card">
        <div className="section-heading">
          <div className="section-icon" aria-hidden="true">⌕</div>
          <div>
            <h2>Encontre seu professor</h2>
            <p>Personalize sua busca para encontrar o melhor horário.</p>
          </div>
        </div>

        <form onSubmit={buscarProfessores}>
          <div className="form-group">
            <label htmlFor="idioma">Qual idioma você quer aprender?</label>
            <select
              id="idioma"
              value={idioma}
              onChange={(event) => setIdioma(event.target.value)}
            >
              <option>Inglês</option>
              <option>Espanhol</option>
              <option>Francês</option>
            </select>
          </div>

          <fieldset className="form-group day-field">
            <legend>Quais dias funcionam para você?</legend>
            <p className="field-hint">
              Selecione todos os dias em que deseja ter aula.
            </p>
            <div className="day-grid">
              {DIAS.map((dia) => {
                const selecionado = dias.includes(dia.valor)

                return (
                  <button
                    key={dia.valor}
                    type="button"
                    className={`day-button ${selecionado ? 'selected' : ''}`}
                    aria-pressed={selecionado}
                    onClick={() => alternarDia(dia.valor)}
                    title={dia.label}
                  >
                    <span className="day-check">
                      {selecionado ? '✓' : ''}
                    </span>
                    {dia.curto}
                  </button>
                )
              })}
            </div>
          </fieldset>

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="inicio">A partir de</label>
              <input
                id="inicio"
                type="time"
                value={inicioFaixa}
                onChange={(event) => setInicioFaixa(event.target.value)}
                required
              />
            </div>
            <div className="form-group">
              <label htmlFor="fim">Até</label>
              <input
                id="fim"
                type="time"
                value={fimFaixa}
                onChange={(event) => setFimFaixa(event.target.value)}
                required
              />
            </div>
          </div>

          <fieldset className="form-group duration-field">
            <legend>Qual a duração da aula?</legend>
            <div className="duration-options">
              {[30, 60, 90].map((minutos) => (
                <button
                  key={minutos}
                  type="button"
                  className={`duration-button ${
                    duracaoMinutos === minutos ? 'selected' : ''
                  }`}
                  aria-pressed={duracaoMinutos === minutos}
                  onClick={() => setDuracaoMinutos(minutos)}
                >
                  <strong>{minutos}</strong>
                  <span>minutos</span>
                </button>
              ))}
            </div>
          </fieldset>

          {erro && (
            <div className="error-message" role="alert">
              <span aria-hidden="true">!</span>
              {erro}
            </div>
          )}

          <button className="submit-button" type="submit" disabled={carregando}>
            {carregando ? (
              <>
                <span className="spinner" />
                Buscando professores...
              </>
            ) : (
              <>
                Buscar professores
                <span aria-hidden="true">→</span>
              </>
            )}
          </button>

          <p className="form-note">
            Busca personalizada · Resultados baseados na disponibilidade
          </p>
        </form>
      </section>

      {resultado && (
        <section className="results-section" aria-live="polite">
          <div className="results-heading">
            <div>
              <span className="eyebrow">SUA BUSCA</span>
              <h2>Resultados encontrados</h2>
              <p>
                {resultado.professores.length === 1
                  ? 'Encontramos 1 professor disponível.'
                  : `Encontramos ${resultado.professores.length} professores disponíveis.`}
              </p>
            </div>
            <span className="results-count">
              {resultado.professores.length} disponíveis
            </span>
          </div>

          {resultado.professores.length > 0 ? (
            <div className="teacher-grid">
              {resultado.professores.map((professor) => (
                <article className="teacher-card" key={professor.id}>
                  <div className="teacher-top">
                    <div className="avatar">
                      {professor.nome.charAt(0).toUpperCase()}
                    </div>
                    <span className="available-tag">
                      <span className="status-dot" />
                      Disponível
                    </span>
                  </div>
                  <h3>{professor.nome}</h3>
                  <p className="teacher-subtitle">
                    {idioma} · Professor(a)
                  </p>
                  <div className="teacher-divider" />
                  <div className="teacher-meta">
                    <span>Aulas agendadas na semana</span>
                    <strong>{professor.aulasNaSemana}</strong>
                  </div>
                  <div className="schedule-title">Horários disponíveis</div>
                  <div className="time-slots">
                    {professor.horarios.map((horario) => (
                      <span className="time-chip" key={horario}>
                        {horario}
                      </span>
                    ))}
                  </div>
                </article>
              ))}
            </div>
          ) : (
            <div className="empty-state">
              <span className="empty-icon">⌕</span>
              <h3>Nenhum professor disponível nesta busca</h3>
              <p>
                Experimente outros dias ou uma faixa de horário diferente.
              </p>
            </div>
          )}

          {resultado.naoElegiveis.length > 0 && (
            <details className="ineligible-section">
              <summary>
                <span>
                  Professores não elegíveis
                  <small>
                    {resultado.naoElegiveis.length} não atendem aos critérios
                    desta busca
                  </small>
                </span>
                <span className="details-arrow">⌄</span>
              </summary>
              <div className="ineligible-list">
                {resultado.naoElegiveis.map((professor) => (
                  <div className="ineligible-row" key={professor.id}>
                    <div className="ineligible-avatar">
                      {professor.nome.charAt(0).toUpperCase()}
                    </div>
                    <strong>{professor.nome}</strong>
                    <span>{professor.motivo}</span>
                  </div>
                ))}
              </div>
            </details>
          )}
        </section>
      )}

      <footer className="footer">
        <span className="footer-brand">find<span>a</span>teacher</span>
        <span>Encontre o horário certo para aprender.</span>
      </footer>
    </main>
  )
}

export default App
