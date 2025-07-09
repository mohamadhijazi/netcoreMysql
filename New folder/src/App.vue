<script setup>
import { ref, computed, onMounted, nextTick, watch } from 'vue'
import entitiesData from './assets/entities_with_dependencies.json'

const dimensionColors = {
  Data: '#4f8cff',
  Time: '#ffb347',
  People: '#4caf50',
  Money: '#e57373',
  default: '#bdbdbd'
}

const dimensions = computed(() => {
  // Extract unique dimension names from all entities
  const set = new Set()
  entitiesData.forEach(e => {
    if (e.Dimension_Levels) {
      Object.keys(e.Dimension_Levels).forEach(d => set.add(d))
    }
  })
  return Array.from(set)
})

const selectedDimension = ref(dimensions.value[0] || '')
const zoom = ref(1)
const offset = ref({ x: 0, y: 0 })
const dragging = ref(false)
const dragStart = ref({ x: 0, y: 0 })
const panStart = ref({ x: 0, y: 0 })
const selectedEntity = ref(null)
const entityPositions = ref({})

// --- CLUSTERING LOGIC ---
// Compute clusters based on zoom and selected dimension
const clusters = computed(() => {
  // For demo: at low zoom, group by dimension level; at high zoom, show individual entities
  if (!selectedDimension.value) return []
  const z = zoom.value
  let clusterMap = {}
  if (z < 1.2) {
    // Group by dimension level
    entitiesData.forEach(e => {
      const level = e.Dimension_Levels?.[selectedDimension.value] || 'Other'
      if (!clusterMap[level]) clusterMap[level] = []
      clusterMap[level].push(e)
    })
  } else if (z < 2) {
    // Group by dimension level + first letter of name (simulate finer clusters)
    entitiesData.forEach(e => {
      const level = e.Dimension_Levels?.[selectedDimension.value] || 'Other'
      const key = level + '-' + (e.Name[0] || '')
      if (!clusterMap[key]) clusterMap[key] = []
      clusterMap[key].push(e)
    })
  } else {
    // Show each entity as its own cluster
    entitiesData.forEach(e => {
      clusterMap[e.Name] = [e]
    })
  }
  // Convert to array of { key, entities }
  return Object.entries(clusterMap).map(([key, entities]) => ({ key, entities }))
})

// --- DEBUG: LOG CLUSTERS ---
watch(clusters, (val) => { console.log('Clusters:', val) })

// --- CLUSTER LAYOUT ---
const clusterPositions = computed(() => {
  // Arrange clusters in a grid
  const pos = {}
  let x = 100, y = 100, col = 0
  clusters.value.forEach((c, i) => {
    pos[c.key] = { x, y }
    x += 350
    col++
    if (col % 4 === 0) { x = 100; y += 250 }
  })
  return pos
})

// --- CLUSTER CARD AGGREGATION ---
function getClusterAttributes(cluster) {
  // Union of all Quantitative_Attributes in the cluster
  const attrSet = new Set()
  cluster.entities.forEach(e => (e.Quantitative_Attributes || []).forEach(a => attrSet.add(a)))
  return Array.from(attrSet)
}
function aggregateClusterAttribute(cluster, attr) {
  // Aggregate attribute across all entities in the cluster
  let sum = 0, count = 0,values = []
  cluster.entities.forEach(entity => {
    const val = aggregateAttribute(entity, attr)
    if (typeof val === 'number') { sum += val; count++ 
     } else if (Array.isArray(val)) {
          values.push(val.join(', '))
        } else if (typeof val === 'string') {
          values.push(val)
        }
  });
  if (count > 0) return sum
  if (values.length > 0) return values.join(', ')
  return count > 0 ? sum : '-'
}

// --- CLUSTER COLOR ---
function getClusterColorByKey(cluster) {
  // Use the first entity's dimension level for color
  const e = cluster.entities[0]
  if (!selectedDimension.value) return dimensionColors.default
  const level = e.Dimension_Levels?.[selectedDimension.value]
  if (level) {
    return dimensionColors[selectedDimension.value] || dimensionColors.default
  }
  return dimensionColors.default
}

function onWheel(e) {
  e.preventDefault()
  zoom.value = Math.max(0.5, Math.min(zoom.value + e.deltaY * -0.001, 3))
}
function onMouseDown(e) {
  dragging.value = true
  dragStart.value = { x: e.clientX, y: e.clientY }
  panStart.value = { ...offset.value }
}
function onMouseMove(e) {
  if (!dragging.value) return
  offset.value = {
    x: panStart.value.x + (e.clientX - dragStart.value.x),
    y: panStart.value.y + (e.clientY - dragStart.value.y)
  }
}
function onMouseUp() {
  dragging.value = false
}

onMounted(() => {
  window.addEventListener('mousemove', onMouseMove)
  window.addEventListener('mouseup', onMouseUp)
  nextTick(() => layoutEntities())
})

function layoutEntities() {
  // Simple grid layout for demo; can be improved for clustering
  const pos = {}
  let x = 100, y = 100, col = 0
  entitiesData.forEach((e, i) => {
    pos[e.Name] = { x, y }
    x += 350
    col++
    if (col % 4 === 0) { x = 100; y += 250 }
  })
  entityPositions.value = pos
}

function aggregateAttribute(entity, attr, visited = new Set()) {
  // Prevent cycles
  if (visited.has(entity.Name)) return 0
  visited.add(entity.Name)
  let sum = 0, count = 0, values = []
  if (entity.childRowData && entity.childRowData.length > 0) {
    entity.childRowData.forEach(row => {
      let v = row[attr]
      if (v != null) {
         if (typeof v === 'string' && !isNaN(Number(v))) {
           v = Number(v)
      }
        if (typeof v === 'number' && !isNaN(v)) {
          sum += v; count++
        } else if (Array.isArray(v)) {
          values.push(v.join(', '))
        } else if (typeof v === 'string') {
          values.push(v)
        }
      }
    })
  }
  // Aggregate from dependencies
  if (entity.Dependencies) {
    entity.Dependencies.forEach(dep => {
      const depEntity = entitiesData.find(e => e.Name === dep.Name)
      if (depEntity) {
        const depVal = aggregateAttribute(depEntity, attr, visited)
        if (typeof depVal === 'number' && !isNaN(depVal)) {
          sum += depVal; count++
        } else if (typeof depVal === 'string' && depVal !== '-') {
          values.push(depVal)
        }
      }
    })
  }
  if (count > 0) return sum
  if (values.length > 0) return values.join(', ')
  return '-'
}

function openEntity(entity) {
  selectedEntity.value = entity
}
function closeEntity() {
  selectedEntity.value = null
}

// --- FLATTENED DEPENDENCY LINES FOR SVG ---
const dependencyLines = computed(() => {
  // Each line: { fromKey, toKey }
  const lines = []
  clusters.value.forEach(cluster => {
    cluster.entities.forEach(entity => {
      (entity.Dependencies || []).forEach(dep => {
        // Find the cluster containing the dependency target
        const toCluster = clusters.value.find(c => c.entities.some(e => e.Name === dep.Name))
        if (toCluster) {
          lines.push({
            fromKey: cluster.key,
            toKey: toCluster.key,
            fromEntity: entity.Name,
            toEntity: dep.Name
          })
        }
      })
    })
  })
  return lines
})

// --- MINIMAP LOGIC ---
const minimapScale = 0.12
const minimapWidth = 2000 * minimapScale
const minimapHeight = 2000 * minimapScale
const minimapViewport = computed(() => {
  // Calculate the visible area of the canvas in minimap coordinates
  const area = document.querySelector('.canvas-area')
  if (!area) return { x: 0, y: 0, w: 0, h: 0 }
  const rect = area.getBoundingClientRect()
  const w = rect.width / zoom.value * minimapScale
  const h = rect.height / zoom.value * minimapScale
  const x = -offset.value.x * minimapScale
  const y = -offset.value.y * minimapScale
  return { x, y, w, h }
})
function minimapGoTo(e) {
  // Move viewport center to clicked minimap position
  const mx = e.offsetX / minimapScale
  const my = e.offsetY / minimapScale
  const area = document.querySelector('.canvas-area')
  if (!area) return
  const rect = area.getBoundingClientRect()
  offset.value = {
    x: -(mx - rect.width / (2 * zoom.value)),
    y: -(my - rect.height / (2 * zoom.value))
  }
}

// --- CLUSTER CARD ATTRIBUTES (limit to 5) ---
function getClusterAttributesLimited(cluster) {
  const attrs = getClusterAttributes(cluster)
  return attrs.slice(0, 5)
}

</script>

<template>
  <div class="app-container">
    <aside class="side-panel">
      <h2>Dimensions</h2>
      <ul>
        <li v-for="dim in dimensions" :key="dim">
          <button :class="{ active: selectedDimension === dim }" @click="selectedDimension = dim">{{ dim }}</button>
        </li>
      </ul>
      <div class="legend">
        <h3>Legend</h3>
        <div v-for="(color, dim) in dimensionColors" v-if="dim !== 'default'" :key="dim" class="legend-row">
          <span class="legend-color" :style="{ background: color }"></span> <span class="legend-label">{{ dim }}</span>
        </div>
      </div>
    </aside>
    <main class="canvas-area">
      <div class="canvas-wrapper">
        <svg
          class="dependency-lines"
          width="2000" height="2000"
          :style="{ transform: `scale(${zoom}) translate(${offset.x / zoom}px, ${offset.y / zoom}px)` }"
        >
          <line
            v-for="line in dependencyLines"
            :key="line.fromEntity + '-' + line.toEntity"
            :x1="(clusterPositions[line.fromKey]?.x ?? entityPositions[line.fromKey]?.x ?? 0) + 130"
            :y1="(clusterPositions[line.fromKey]?.y ?? entityPositions[line.fromKey]?.y ?? 0) + 60"
            :x2="(clusterPositions[line.toKey]?.x ?? entityPositions[line.toKey]?.x ?? 0) + 130"
            :y2="(clusterPositions[line.toKey]?.y ?? entityPositions[line.toKey]?.y ?? 0) + 60"
            stroke="#888" stroke-width="2" marker-end="url(#arrow)"/>
          <defs>
            <marker id="arrow" markerWidth="10" markerHeight="10" refX="10" refY="5" orient="auto" markerUnits="strokeWidth">
              <path d="M0,0 L10,5 L0,10 Z" fill="#888" />
            </marker>
          </defs>
        </svg>
        <div
          class="canvas"
          :style="{ transform: `scale(${zoom}) translate(${offset.x / zoom}px, ${offset.y / zoom}px)` }"
          @wheel="onWheel"
          @mousedown="onMouseDown"
        >
          <transition-group name="cluster-move" tag="div">
            <div
              v-if="clusters.length === 0"
              v-for="entity in entitiesData"
              :key="entity.Name"
              class="cluster-card"
              :style="{ left: entityPositions[entity.Name]?.x + 'px', top: entityPositions[entity.Name]?.y + 'px', borderColor: getClusterColor(entity), transition: 'left 0.3s, top 0.3s' }"
              @click="openEntity(entity)"
            >
              <div v-for="(attr, i) in entity.Quantitative_Attributes.slice(0, 5)" :key="attr" class="attr-row">
                <span>{{ attr }}:</span>
                <span>{{ aggregateAttribute(entity, attr) }}</span>
              </div>
              <div v-if="entity.Quantitative_Attributes.length > 5" class="attr-row"><span>…</span></div>
            </div>
            <div
              v-else
              v-for="cluster in clusters"
              :key="cluster.key"
              class="cluster-card"
              :style="{ left: clusterPositions[cluster.key]?.x + 'px', top: clusterPositions[cluster.key]?.y + 'px', borderColor: getClusterColorByKey(cluster), transition: 'left 0.3s, top 0.3s' }"
              @click="openEntity(cluster.entities[0])"
            >
            <span>{{ cluster.entities[0].Name }}:</span>
              <div v-for="(attr, i) in getClusterAttributesLimited(cluster)" :key="attr" class="attr-row">
                <span>{{ attr }}:</span>
                <span>{{ aggregateClusterAttribute(cluster, attr) }}</span>
              </div>
              <div v-if="getClusterAttributes(cluster).length > 5" class="attr-row"><span>…</span></div>
            </div>
          </transition-group>
        </div>
      </div>
      <!-- Move zoom controls and minimap OUTSIDE the canvas and wrapper -->
      <div class="zoom-controls-fixed">
        <button class="zoom-btn" @click="zoom = Math.min(zoom + 0.2, 3)">+</button>
        <button class="zoom-btn" @click="zoom = Math.max(zoom - 0.2, 0.5)">-</button>
      </div>
      <div class="minimap-fixed" :style="{ width: minimapWidth + 'px', height: minimapHeight + 'px' }" @click="minimapGoTo">
        <div
          v-for="cluster in clusters.length ? clusters : entitiesData.map(e => ({ key: e.Name, entities: [e] }))"
          :key="cluster.key"
          class="minimap-dot"
          :style="{
            left: (clusterPositions[cluster.key]?.x || entityPositions[cluster.key]?.x) * minimapScale + 8 + 'px',
            top: (clusterPositions[cluster.key]?.y || entityPositions[cluster.key]?.y) * minimapScale + 8 + 'px',
            background: getClusterColorByKey ? getClusterColorByKey(cluster) : getClusterColor(cluster.entities[0])
          }"
        ></div>
        <div class="minimap-viewport" :style="{
          left: minimapViewport.x + 'px',
          top: minimapViewport.y + 'px',
          width: minimapViewport.w + 'px',
          height: minimapViewport.h + 'px'
        }"></div>
      </div>
      <div v-if="selectedEntity" class="entity-modal" @click.self="closeEntity">
        <div class="modal-content">
          <h2>{{ selectedEntity.Name }}</h2>
          <div v-for="attr in selectedEntity.Quantitative_Attributes" :key="attr">
            <b>{{ attr }}:</b> {{ aggregateAttribute(selectedEntity, attr) }}
          </div>
          <h4>Dependencies</h4>
          <ul>
            <li v-for="dep in selectedEntity.Dependencies || []" :key="dep.Name">
              {{ dep.Relation }} → {{ dep.Name }}
            </li>
          </ul>
          <button @click="closeEntity">Close</button>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
.app-container {
  height: 100vh;
  min-width: 0;
}
.side-panel {
  width: 240px;
  background: #f8f9fa;
  border-right: 1px solid #ddd;
  padding: 1rem;
  overflow-y: auto;
  min-width: 180px;
  color: #222;
  position: fixed;
  left: 0;
  top: 0;
  bottom: 0;
  z-index: 100;
  height: 100vh;
  box-sizing: border-box;
}
.canvas-area {
  margin-left: 240px;
  position: relative;
  background: #f0f4f8;
  min-width: 0;
  height: 100vh;
  overflow: auto;
  display: block;
}
.canvas-wrapper {
  position: relative;
  width: 100%;
  height: 100%;
  overflow: auto;
  display: block;
}
.canvas {
  width: 2000px;
  height: 2000px;
  position: relative;
  transition: transform 0.2s;
  color: #222;
  background: #fff;
  border: 2px dashed #bbb;
  z-index: 2;
  display: block;
  cursor: grab;
}
.canvas:active {
  cursor: grabbing;
}
.dependency-lines {
  width: 2000px;
  height: 2000px;
  position: absolute;
  pointer-events: none;
  z-index: 6;
  display: block;
}
.entity-modal {
  position: fixed;
  left: 0; top: 0; right: 0; bottom: 0;
  background: rgba(0,0,0,0.35);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 10;
  backdrop-filter: blur(2px);
}
.modal-content {
  background: #fff;
  border-radius: 12px;
  padding: 2rem 2.5rem;
  min-width: 320px;
  max-width: 90vw;
  color: #222;
  box-shadow: 0 8px 32px rgba(0,0,0,0.18);
}
.cluster-card {
  position: absolute;
  min-width: 180px;
  max-width: 260px;
  background: #fff;
  border: 3px solid #bbb;
  border-radius: 14px;
  box-shadow: 0 2px 12px rgba(80,120,180,0.08);
  padding: 1rem 1.2rem 0.7rem 1.2rem;
  cursor: pointer;
  transition: box-shadow 0.2s, transform 0.2s, border-color 0.2s, left 0.3s, top 0.3s;
  z-index: 3;
}
.cluster-card:hover {
  box-shadow: 0 6px 24px rgba(80,120,180,0.18);
  transform: scale(1.04);
  border-color: #222;
}
.attr-row {
  display: flex;
  justify-content: space-between;
  font-size: 0.98em;
  margin-bottom: 0.2em;
}
.legend {
  margin-top: 2.5rem;
}
.legend-row {
  display: flex;
  align-items: center;
  margin-bottom: 0.5em;
}
.legend-color {
  width: 18px;
  height: 18px;
  border-radius: 4px;
  margin-right: 0.7em;
  border: 1px solid #bbb;
  display: inline-block;
}
.legend-label {
  font-size: 1em;
}
button {
  background: #f0f4f8;
  border: 1px solid #bbb;
  border-radius: 6px;
  padding: 0.4em 1.1em;
  margin-bottom: 0.5em;
  cursor: pointer;
  font-size: 1em;
  transition: background 0.15s, border 0.15s;
}
button.active, button:active {
  background: #4f8cff;
  color: #fff;
  border-color: #4f8cff;
}
.zoom-controls {
  display: none;
}
.zoom-controls-fixed {
  position: fixed;
  right: 32px;
  bottom: 32px;
  z-index: 120;
  display: flex;
  flex-direction: column;
  gap: 0.7em;
}
.minimap {
  display: none;
}
.minimap-fixed {
  position: fixed;
  left: 24px;
  bottom: 24px;
  background: rgba(255,255,255,0.95);
  border: 1.5px solid #bbb;
  border-radius: 10px;
  box-shadow: 0 2px 8px rgba(80,120,180,0.10);
  z-index: 120;
  overflow: hidden;
  width: 240px;
  height: 240px;
  min-width: 120px;
  min-height: 120px;
  max-width: 320px;
  max-height: 320px;
  cursor: pointer;
}
.minimap-dot {
  position: absolute;
  width: 14px;
  height: 14px;
  border-radius: 50%;
  border: 2px solid #fff;
  box-shadow: 0 1px 4px rgba(80,120,180,0.10);
  z-index: 2;
}
.minimap-viewport {
  position: absolute;
  border: 2px solid #4f8cff;
  border-radius: 4px;
  background: rgba(79,140,255,0.08);
  z-index: 3;
  pointer-events: none;
}
</style>
